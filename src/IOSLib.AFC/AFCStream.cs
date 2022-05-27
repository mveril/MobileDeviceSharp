using IOSLib.AFC.Native;
using System;
using System.IO;
using System.Runtime.InteropServices;
using static IOSLib.AFC.Native.AFC;

namespace IOSLib.AFC
{
    public class AFCStream : Stream
    {

        public AFCSessionBase Session { get;  }

        private readonly string _path;
        private ulong _fHandle;

        public AFCStream(AFCSessionBase session, string path, FileMode mode, FileAccess access, AFCLockOp fileLock)
        {
            var file = new AFCFile(session, path);
            Session = session;
            _path = path;
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
            var hresult = afc_file_open(Session.Handle, path, AFCMode, out _fHandle);
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
                afc_file_truncate(session.Handle, _fHandle, 0);
            }
        }

        public void Lock(AFCLockOp operation)
        {
            var hresult = afc_file_lock(Session.Handle, _fHandle, operation);
            if (hresult.IsError())
                throw hresult.GetException();
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
                return long.Parse(Session.GetFileInfo(_path)["st_size"]);
            }
        }

        public override long Position
        {
            get
            {
                ValidateHandle();
                var hresult = afc_file_tell(Session.Handle, _fHandle, out var position);
                if (hresult.IsError())
                    throw  new IOException("An exception occure when we triy to get the stream position",hresult.GetException());
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
            var offsetbuffer = new ArrayWithOffset(buffer, offset);
            var hresult = afc_file_read(Session.Handle, _fHandle, offsetbuffer, (uint)count, out var byteread);
            if (hresult.IsError())
                throw new IOException("Read operation failed", hresult.GetException());
            return (int)byteread;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ValidateHandle();
            var hresult = afc_file_seek(Session.Handle, _fHandle, offset, (SeekWhence)(int)origin); // SeekOrigin values are the same as standard Unix seek values.
            if(hresult.IsError())
                throw new IOException("Seek operation failed.", hresult.GetException());
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

                    var hresult = afc_file_truncate(Session.Handle, _fHandle, (ulong)value);
                    if (hresult.IsError())
                        throw new IOException("Truncate operation failed", hresult.GetException());
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

            var offsetbuffer = new ArrayWithOffset(buffer, offset);
            var hresult = afc_file_write(Session.Handle, _fHandle, offsetbuffer, (uint)count, out _);
            if (hresult.IsError())
                throw new IOException("Write operation failed.", hresult.GetException());
        }

        private  void ValidateHandle()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("The file is closed");
            }
            else if (Session.IsClosed)
            {
                throw new ObjectDisposedException("Session is closed");
            }
        }

        private bool _isDisposed = false;


        protected override void Dispose(bool disposing)
        {
            ValidateHandle();
            afc_file_close(Session.Handle, _fHandle);
            _isDisposed = true;
            _fHandle = 0;
            base.Dispose(disposing);
        }
    }
}
