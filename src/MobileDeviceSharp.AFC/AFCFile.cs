using MobileDeviceSharp.AFC.Native;
using System;
using System.Collections.Generic;
using System.IO;
using static MobileDeviceSharp.AFC.Native.AFC;
using System.Text;

namespace MobileDeviceSharp.AFC
{
    /// <summary>
    /// Represents a file in the AFC file system.
    /// </summary>
    public sealed class AFCFile : AFCItem
    {
       internal AFCFile(AFCSessionBase session, string path) : base(session, path) { }

        /// <inheritdoc/>
        protected override bool IsItemTypeSupported(AFCItemType itemType)
        {
            return itemType == AFCItemType.File;
        }

        /// <summary>
        /// Gets the length of the file in byte.
        /// </summary>
        public long Length => long.Parse(GetFileInfo()["st_size"]);


        /// <summary>
        /// Opens the file for appending text.
        /// </summary>
        /// <returns>A <see cref="StreamWriter"/> object that can be used to append text to the file.</returns>
        public StreamWriter AppendText()
        {
            var stream = new AFCStream(Session, Path, FileMode.Append, FileAccess.Write, AFCLockOp.LockEx);
            return new StreamWriter(stream);
        }

        /// <summary>
        /// Creates a new file.
        /// </summary>
        /// <returns>An <see cref="AFCStream"/> object that can be used to write to the file.</returns>
        public AFCStream Create()
        {
            return new AFCStream(Session, Path, FileMode.CreateNew, FileAccess.Write, AFCLockOp.LockEx);
        }

        /// <summary>
        /// Creates a new file and returns a <see cref="StreamWriter"/> object that can be used to write text to the file.
        /// </summary>
        /// <returns>A <see cref="StreamWriter"/> object that can be used to write text to the file.</returns>
        public StreamWriter CreateText()
        {
            return new StreamWriter(Create());
        }

        /// <summary>
        /// Opens a file for reading, writing, or both.
        /// </summary>
        /// <param name="mode">The mode in which to open the file.</param>
        /// <param name="access">The access rights to the file.</param>
        /// <param name="lock">The type of lock to place on the file.</param>
        /// <returns>An <see cref="AFCStream"/> object that can be used to access the file.</returns>
        public AFCStream Open(FileMode mode, FileAccess access, AFCLockOp @lock)
        {
            return new AFCStream(Session, Path, mode, access, @lock);
        }

        /// <summary>
        /// Opens a file for reading.
        /// </summary>
        /// <returns>An <see cref="AFCStream"/> object that can be used to read from the file.</returns>
        public AFCStream OpenRead()
        {
            return Open(FileMode.Open, FileAccess.Read, AFCLockOp.LockSh);
        }

        /// <summary>
        /// Opens a text file for reading.
        /// </summary>
        /// <returns>A <see cref="StreamReader"/> object that can be used to read text from the file.</returns>
        public StreamReader OpenText()
        {
            return new StreamReader(OpenRead());
        }

        /// <summary>
        /// Opens a file for writing.
        /// </summary>
        /// <returns>An <see cref="AFCStream"/> object that can be used to write to the file.</returns>
        public AFCStream OpenWrite()
        {
            return Open(FileMode.Truncate, FileAccess.ReadWrite, AFCLockOp.LockEx);
        }
    }
}
