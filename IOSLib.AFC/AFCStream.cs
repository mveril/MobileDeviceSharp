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
        private ulong fHandle = 0;

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
            var ex = afc_file_open(Session.Handle, path, AFCMode, ref fHandle).GetException();
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
                ulong var = 0;
                var ex = afc_file_tell(Session.Handle, fHandle, ref var).GetException();
                if (ex != null)
                    throw ex;
                return (long)var;
            }

            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override void Flush()
        {

        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            uint byteread = 0;
            if (offset == 0)
            {
                var ex = afc_file_read(Session.Handle, fHandle, buffer, (uint)count, ref byteread).GetException();
                if (ex != null)
                    throw ex;
            }
            else
            {
                var resultSpan = new Span<byte>(buffer, offset, count);
                unsafe
                {
                    Exception ex;
                    fixed (byte* bptr = resultSpan)
                    {
                        ex = afc_file_read(Session.Handle, fHandle, bptr, (uint)count, ref byteread).GetException();
                    }
                    if (ex != null)
                        throw ex;
                }
            }

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
            uint bytesWritten = 0;
            if (offset == 0)
            {
                var ex = afc_file_write(Session.Handle, fHandle, buffer, (uint)count, ref bytesWritten).GetException();
                if (ex != null)
                    throw ex;
            }
            else
            {
                var targetSpan = new Span<byte>(buffer, offset, count);
                Exception ex;
                unsafe
                {
                    fixed(byte* bptr = targetSpan)
                    {
                        ex = afc_file_write(Session.Handle, fHandle, bptr, (uint)count, ref bytesWritten).GetException();
                    }
                }
                if (ex != null)
                    throw ex;
            }
        }

        protected override void Dispose(bool disposing)
        {
            afc_file_close(Session.Handle, fHandle);
            base.Dispose(disposing);
        }
    }
}