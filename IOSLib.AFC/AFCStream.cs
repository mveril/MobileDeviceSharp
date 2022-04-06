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
            Session = session;
            this.path = path;
            FileAccess = access;
            AFCFileMode AFCMode = (access, mode) switch
            {
                (FileAccess.Read, FileMode.Open) => AFCFileMode.FopenRdonly, // r
                (FileAccess.ReadWrite, FileMode.OpenOrCreate) => AFCFileMode.FopenRw, //r+
                (FileAccess.Write, FileMode.Truncate) => AFCFileMode.FopenWronly, //w
                (FileAccess.ReadWrite, FileMode.Truncate) => AFCFileMode.FopenWr, //w+
                (FileAccess.Write, FileMode.Append) => AFCFileMode.FopenAppend, //a
                (FileAccess.ReadWrite, FileMode.Append) => AFCFileMode.FopenRdappend, //a+
                _ => throw new InvalidOperationException(),
            };
            var ex = afc_file_open(Session.Handle, path, AFCMode, out fHandle).GetException();
            if (ex != null)
                throw ex;
            Lock(fileLock);
        }

        public void Lock(AFCLockOp operation)
        {
            var ex = afc_file_lock(Session.Handle, fHandle, operation).GetException();
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
                return long.Parse(Session.GetFileInfo(path)["st_size"]);
            }
        }

        public override long Position
        {
            get
            {
                var ex = afc_file_tell(Session.Handle, fHandle, out var position).GetException();
                if (ex != null)
                    throw ex;
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
            if (offset==0)
            {
                return ReadInternal(buffer, count);
            }
            else
            {
                var targetSpan= new Span<byte>(buffer, offset, count);
                return ReadInternal(targetSpan);
            }
        }

        private int ReadInternal(byte[] buffer, int count)
        {
            Exception ex;
            ex = afc_file_read(Session.Handle, fHandle, buffer, (uint)count, out var byteread).GetException();
            if (ex != null)
                throw ex;
            return (int)byteread;
        }

        private int ReadInternal(Span<byte> buffer)
        {
            Exception ex;
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
            var ex = afc_file_seek(Session.Handle, fHandle, values.Item1, values.Item2).GetException();
            if (ex != null)
                throw ex;
            return Position;
        }

        public override void SetLength(long value)
        {
            var ex = afc_file_truncate(Session.Handle, fHandle, (ulong)value).GetException();
            if (ex != null)
                throw ex;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            if (offset==0)
            {
                WriteInternal(buffer, count);
            }
            else
            {
                var targetSpan = new ReadOnlySpan<byte>(buffer, offset, count);
                WriteInternal(targetSpan);
            }
        }

        public void WriteInternal(byte[] buffer, int count)
        {
            Exception ex = afc_file_write(Session.Handle, fHandle, buffer, (uint)count, out _).GetException();
            if (ex != null)
                throw ex;
        }

        public void WriteInternal(ReadOnlySpan<byte> buffer)
        {
            Exception ex;
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

        protected override void Dispose(bool disposing)
        {
            afc_file_close(Session.Handle, fHandle);
            base.Dispose(disposing);
        }
    }
}