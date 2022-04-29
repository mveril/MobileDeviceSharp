using IOSLib.AFC.Native;
using System;
using System.Collections.Generic;
using System.IO;
using static IOSLib.AFC.Native.AFC;
using System.Text;

namespace IOSLib.AFC
{
    public class AFCFile : AFCItem
    {
       internal AFCFile(AFCSessionBase session, string path) : base(session, path) { }

        protected override bool IsItemTypeSupported(AFCItemType itemType)
        {
            return itemType == AFCItemType.File;
        }

        public long Length => long.Parse(GetFileInfo()["st_size"]);

        public StreamWriter AppendText()
        {
            var stream = new AFCStream(Session, this.Path, FileMode.Append, FileAccess.Write, AFCLockOp.LockEx);
            return new StreamWriter(stream);
        }
        public AFCStream Create()
        {
            return new AFCStream(Session, Path, FileMode.CreateNew, FileAccess.Write, AFCLockOp.LockEx);
        }

        public StreamWriter CreateText()
        {
            return new StreamWriter(Create());
        }

        public AFCStream Open(FileMode mode, FileAccess access, AFCLockOp @lock)
        {
            return new AFCStream(Session, Path, mode, access, @lock);
        }

        public AFCStream OpenRead()
        {
            return Open(FileMode.Open, FileAccess.Read, AFCLockOp.LockSh);
        }
        public StreamReader OpenText()
        {
            return new StreamReader(OpenRead());
        }

        public AFCStream OpenWrite()
        {
            return Open(FileMode.Truncate, FileAccess.ReadWrite, AFCLockOp.LockEx);
        }
    }
}
