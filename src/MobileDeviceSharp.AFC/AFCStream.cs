using MobileDeviceSharp.AFC.Native;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static MobileDeviceSharp.AFC.Native.AFC;

namespace MobileDeviceSharp.AFC
{
    /// <summary>
    /// Represents a Stream object that can be used to read from or write to a file on an Apple device using AFC.
    /// </summary>
    public sealed class AFCStream : Stream
    {
        /// <summary>
        /// Gets the <see cref="AFCSessionBase"/> object associated with this stream.
        /// </summary>
        public AFCSessionBase Session { get;  }

        private readonly string _path;
        private ulong _fHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="AFCStream"/> class with the specified <see cref="AFCSessionBase"/> and file path, mode, access, and lock operation.
        /// </summary>
        /// <param name="session">The <see cref="AFCSessionBase"/> object associated with this stream.</param>
        /// <param name="path">The file path on the device to be associated with this stream.</param>
        /// <param name="fileMode">The <see cref="FileMode"/> to use when opening or creating the file on the device.</param>
        /// <param name="fileAccess">The <see cref="FileAccess"/> mode to use when accessing the file on the device.</param>
        /// <param name="fileLock">The <see cref="AFCLockOp"/> to apply to the file on the device when opening the stream.</param>
        /// <exception cref="NotSupportedException">You try to use an unsupported value of <see cref="FileMode"/> (<see cref="FileMode.CreateNew"/> or <see cref="FileMode.Truncate"/> that is not supported by AppleFileConduit.</exception>
        /// <exception cref="ArgumentException">You use an invalid combinaison of <see cref="FileAccess"/> and <see cref="FileMode"/></exception>
        public AFCStream(AFCSessionBase session, string path, FileMode fileMode, FileAccess fileAccess, AFCLockOp fileLock)
        {
            Session = session;
            _path = path;
            _fileAccess = fileAccess;
            AFCFileMode AFCMode = (fileMode, fileAccess) switch
            {
                (FileMode.Open, FileAccess.Read) => AFCFileMode.FopenRdonly, // r
                (FileMode.OpenOrCreate, FileAccess.ReadWrite) => AFCFileMode.FopenRw, // r+
                (FileMode.Create, FileAccess.Write) => AFCFileMode.FopenWronly, // w
                (FileMode.Create, FileAccess.ReadWrite) => AFCFileMode.FopenWr, // w+
                (FileMode.Append, FileAccess.Write) => AFCFileMode.FopenAppend, // a
                (FileMode.Append, FileAccess.ReadWrite) => AFCFileMode.FopenRdappend, // "a+"
                (FileMode.CreateNew or FileMode.Truncate, _) => throw new NotSupportedException($"The {nameof(FileMode)} {fileMode} is not supported by AFC"),
                _ => throw new ArgumentException($"Invalid combination of {fileMode} and {fileAccess}.")
            };
            var hresult = afc_file_open(Session.Handle, path, AFCMode, out _fHandle);
            if (hresult.IsError())
                throw hresult.GetException().ToStandardException(AFCItemType.File, _path);
                Lock(fileLock);
        }

        /// <summary>
        /// Locks the file associated with this stream with the specified <see cref="AFCLockOp"/> operation.
        /// </summary>
        /// <param name="operation">The <see cref="AFCLockOp"/> operation to apply to the file.</param>
        public void Lock(AFCLockOp operation)
        {
            var hresult = afc_file_lock(Session.Handle, _fHandle, operation);
            if (hresult.IsError())
                throw hresult.GetException().ToStandardException(AFCItemType.File, _path);
        }

        private readonly FileAccess _fileAccess;

        /// <inheritdoc/>
        public override bool CanRead => (!Session?.IsClosed).GetValueOrDefault(false) && _fileAccess.HasFlag(FileAccess.Read);

        /// <inheritdoc/>
        public override bool CanSeek => (!Session?.IsClosed).GetValueOrDefault(false);

        /// <inheritdoc/>
        public override bool CanWrite => (!Session?.IsClosed).GetValueOrDefault(false) && _fileAccess.HasFlag(FileAccess.Write);

        /// <inheritdoc/>
        public override long Length
        {
            get
            {
                ValidateHandle();
                return long.Parse(Session.GetFileInfo(_path)["st_size"]);
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override void Flush()
        {

        }

        /// <inheritdoc/>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
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

        private unsafe int ReadCore(Span<byte> buffer)
        {
            ValidateHandle();
            if (!CanRead)
            {
                throw new NotSupportedException("This stream is readonly");
            }
            AFCError hresult;
            uint byteread;
            fixed (byte* b = buffer)
            {
                hresult = afc_file_read(Session.Handle, _fHandle, b, (uint)buffer.Length, out byteread);
                if (hresult.IsError())
                    throw new IOException("Read operation failed.", hresult.GetException());
            }
            return (int)byteread;
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            return ReadCore(new Span<byte>(buffer, offset, count));
        }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        /// <inheritdoc/>
        public override int Read(Span<byte> buffer) => ReadCore(buffer);

        /// <inheritdoc/>
        public async override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (buffer.IsEmpty)
            {
                return 0;
            }
            return await Task.Factory.StartNew(static (state) =>
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                var (stream, buffer) = ((AFCStream state, Memory<byte> buffer))state;
#pragma warning restore CS8605 // Unboxing a possibly null value.
                return stream.ReadCore(buffer.Span);
            }, (this, buffer), cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }
#endif

        /// <inheritdoc/>
        public override unsafe int ReadByte()
        {

            byte b;
            return ReadCore(new Span<byte>(&b, 1)) == 0 ? -1 : b;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            ValidateHandle();
            var hresult = afc_file_seek(Session.Handle, _fHandle, offset, (SeekWhence)(int)origin); // SeekOrigin values are the same as standard Unix seek values.
            if(hresult.IsError())
                throw new IOException("Seek operation failed.", hresult.GetException());
            return Position;
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            if (value<0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            if (!CanWrite)
            {
                throw new NotSupportedException();
            }
            var hresult = afc_file_truncate(Session.Handle, _fHandle, (ulong)value);
            if (hresult.IsError())
                throw new IOException("Truncate operation failed.", hresult.GetException());
        }

        private unsafe void WriteCore(ReadOnlySpan<byte> buffer)
        {
            ValidateHandle();
            if (!CanWrite)
            {
                throw new NotSupportedException();
            }
            unsafe
            {
                fixed (byte* b = buffer)
                {
                    var hresult = afc_file_write(Session.Handle, _fHandle, b, (uint)buffer.Length, out _);
                    if (hresult.IsError())
                        throw new IOException("Write operation failed.", hresult.GetException());
                }
            }
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            WriteCore(new ReadOnlySpan<byte>(buffer , offset, count));
        }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        /// <inheritdoc/>
        public override void Write(ReadOnlySpan<byte> buffer) => WriteCore(buffer);

        /// <inheritdoc/>
        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (buffer.IsEmpty)
            {
                return;
            }
            await Task.Factory.StartNew(static (state) =>
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                var (stream, buffer) = ((AFCStream state, ReadOnlyMemory<byte> buffer))state;
#pragma warning restore CS8605 // Unboxing a possibly null value.
                stream.WriteCore(buffer.Span);
            }, (this, buffer), cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }
#endif

        /// <inheritdoc/>
        public override unsafe void WriteByte(byte value)
        {
            WriteCore(new ReadOnlySpan<byte>(&value, 1));
        }

        private void ValidateHandle()
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

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            ValidateHandle();
            var hresut = afc_file_close(Session.Handle, _fHandle);
            if (hresut.IsError())
                throw hresut.GetException();
            _isDisposed = true;
            _fHandle = 0;
            base.Dispose(disposing);
        }
    }
}
