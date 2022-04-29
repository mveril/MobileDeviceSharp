using IOSLib.AFC.Native;
using System;
using System.IO;
using static IOSLib.AFC.Native.AFC;

namespace IOSLib.AFC
{
    public class AFCStream : Stream
    {

        public AFCSessionBase Session { get;  }

        private readonly string path;
        private ulong fHandle;

        public AFCStream(AFCSessionBase session, string path, FileMode mode, FileAccess access, AFCLockOp fileLock)
        {
            var file = new AFCFile(session, path);
            Session = session;
            this.path = path;
            FileAccess = access;
            var needTruncate = mode == FileMode.Truncate || mode == FileMode.Create;
            bool isNew = false;
            void CreateNew()
            {
                if (!file.Exists)
                {
                    Console.WriteLine(file.Path);
                    Console.WriteLine(file.Parent?.Exists);
                    var err=afc_file_open(session.Handle, path, AFCFileMode.FopenWronly, out ulong tmpfhandle);
                    Console.WriteLine(err.ToString());
                    isNew = true;
                    afc_file_close(session.Handle, tmpfhandle);
                }
            }
            switch (mode)
            {
                case FileMode.Append:
                case FileMode.Open:
                case FileMode.Truncate:
                                        if (!file.Exists)
                        throw new InvalidOperationException();
                    break;
                case FileMode.CreateNew:
                    if (file.Exists)
                        throw new InvalidOperationException();
                    if (access == FileAccess.ReadWrite)
                    {
                        CreateNew();
                    }
                    break;
                case FileMode.Create:
                case FileMode.OpenOrCreate:
                    if (access == FileAccess.ReadWrite)
                    {
                        CreateNew();
                    }
                    break;
                default:
                    break;
            }
            AFCFileMode AFCMode = (access, mode) switch
            {
                (FileAccess.Read, FileMode.Open) => AFCFileMode.FopenRdonly, // r
                (FileAccess.ReadWrite, FileMode.Open or FileMode.Truncate or FileMode.OpenOrCreate) => AFCFileMode.FopenRw, //r+
                (FileAccess.Write, FileMode.Open or FileMode.Truncate or FileMode.CreateNew or FileMode.OpenOrCreate or FileMode.Create) => AFCFileMode.FopenWronly, // w
                (FileAccess.ReadWrite, FileMode.Append) => AFCFileMode.FopenRdappend, //a+
                (FileAccess.Write, FileMode.Append or FileMode.OpenOrCreate) => AFCFileMode.FopenAppend, //a+
                (FileAccess.ReadWrite, FileMode.Create) => AFCFileMode.FopenRw, // rw+  
                _ => throw new InvalidOperationException(),
            };
            var hresult = afc_file_open(Session.Handle, path, AFCMode, out fHandle);
            switch (hresult)
            {
                case AFCError.Success:
                    break;
                case AFCError.ObjectNotFound:
                    throw new UnauthorizedAccessException($"File not found : {path}", hresult.GetException());
                case AFCError.ObjectIsDir:
                    throw new UnauthorizedAccessException($"Object is directory : {path}", hresult.GetException());
                case AFCError.PermDenied:
                    throw new UnauthorizedAccessException($"Permission denied : {path}", hresult.GetException());
                case AFCError.ObjectExists:
                    throw new IOException($"File already exist : {path}", hresult.GetException());
                case AFCError.IoError:
                    throw new IOException("IO error", hresult.GetException());
                default:
                    throw hresult.GetException();
            }
            Lock(fileLock);
            if (!isNew && needTruncate)
            {
                afc_file_truncate(session.Handle, fHandle, 0);
            }
        }

        public void Lock(AFCLockOp operation)
        {
            AFCException ex = afc_file_lock(Session.Handle, fHandle, operation).GetException();
            if (ex != null)
                throw ex;
        }

        public FileAccess FileAccess { get; }
        public override bool CanRead => (!Session?.IsClosed).GetValueOrDefault(false) && FileAccess.HasFlag(FileAccess.Read);

        public override bool CanSeek => (!Session?.IsClosed).GetValueOrDefault(false);

        public override bool CanWrite => (!Session?.IsClosed).GetValueOrDefault(false) && FileAccess.HasFlag(FileAccess.Write);

        public override long Length
        {
            get
            {
                ValidateHandle();
                return long.Parse(Session.GetFileInfo(path)["st_size"]);
            }
        }

        public override long Position
        {
            get
            {
                ValidateHandle();
                var ex = afc_file_tell(Session.Handle, fHandle, out var position).GetException();
                if (ex != null)
                    throw  new IOException("An exception occure when we triy to get the stream position",ex);
                return (long)position;
            }

            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override void Flush()
        {

        }

#if !NET6_0_OR_GREATER
        private static void ValidateBufferArguments(byte[] buffer, int offset, int count)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if ((uint)count > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
        }
#endif

        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            try
            {
                if (offset == 0)
                {
                    return ReadInternal(buffer, count);
                }
                else
                {
                    var targetSpan = new Span<byte>(buffer, offset, count);
                    return ReadInternal(targetSpan);
                }
            }
            catch (AFCException ex)
            {

                throw new IOException("Read operation failed", ex);
            }
        }

        private int ReadInternal(byte[] buffer, int count)
        {
            
            AFCException ex = afc_file_read(Session.Handle, fHandle, buffer, (uint)count, out var byteread).GetException();
            if (ex != null)
                throw ex;
            return (int)byteread;
        }

        private int ReadInternal(Span<byte> buffer)
        {
            AFCException ex;
            uint byteread;
            unsafe
            {
                fixed (byte* bptr = buffer)
                {
                    ex = afc_file_read(Session.Handle, fHandle, bptr, (uint)buffer.Length, out byteread).GetException();
                }
            }
            if (ex != null)
                throw ex;
            return (int)byteread;
        }


        public override long Seek(long offset, SeekOrigin origin)
        {

            var values = origin switch
            {
                SeekOrigin.Begin => (offset, 0),
                SeekOrigin.Current => (offset += Position, 0),
                SeekOrigin.End => (offset, 1)
            };
            ValidateHandle();
            AFCException ex = afc_file_seek(Session.Handle, fHandle, values.Item1, values.Item2).GetException();
            if (ex != null)
                throw new IOException("Seek operation failed.", ex);
            return Position;
        }

        public override void SetLength(long value)
        {
            if (value<0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            else
            { 
                if (CanWrite)
                {

                    AFCException? ex = afc_file_truncate(Session.Handle, fHandle, (ulong)value).GetException();
                    if (ex != null)
                        throw new IOException("Truncate operation failed", ex);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            ValidateHandle();
            try
            {
                if (offset == 0)
                {
                    WriteInternal(buffer, count);
                }
                else
                {
                    var targetSpan = new ReadOnlySpan<byte>(buffer, offset, count);
                    WriteInternal(targetSpan);
                }
            }
            catch (AFCException ex)
            {
                throw new IOException("Write operation failed.", ex);
            }
        }

        public void WriteInternal(byte[] buffer, int count)
        {
            AFCException ex = afc_file_write(Session.Handle, fHandle, buffer, (uint)count, out _).GetException();
            if (ex != null)
                throw ex;
        }

        public void WriteInternal(ReadOnlySpan<byte> buffer)
        {
            AFCException ex;
            unsafe
            {
                fixed (byte* bptr = buffer)
                {
                    ex = afc_file_write(Session.Handle, fHandle, bptr, (uint)buffer.Length, out _).GetException();
                }
            }
            if (ex != null)
                throw ex;
        }
        private  void ValidateHandle()
        {
            if (_IsDisposed)
            {
                throw new ObjectDisposedException("The file is closed");
            }
            else if (Session.IsClosed)
            {
                throw new ObjectDisposedException("Session is closed");
            }
        }

        private bool _IsDisposed = false;


        protected override void Dispose(bool disposing)
        {
            ValidateHandle();
            afc_file_close(Session.Handle, fHandle);
            _IsDisposed = true;
            fHandle = 0;
            base.Dispose(disposing);
        }
    }
}
