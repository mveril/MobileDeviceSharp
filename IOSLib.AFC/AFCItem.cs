using System;
using System.Collections.Generic;
using System.Text;
using static IOSLib.AFC.Native.AFC;
using IOSLib.AFC.Native;

namespace IOSLib.AFC
{
    public abstract class AFCItem
    {

        public AFCItemType ItemType => GetItemType();

        internal IReadOnlyDictionary<string, string> GetFileInfo()
        {
            return Session.GetFileInfo(this.Path);
        }

        internal AFCItemType GetItemType()
        {
            return Session.GetItemType(this.Path);
        }


        protected abstract bool IsItemTypeSupported(AFCItemType itemType);

        public string Path { get; protected set; }

        const string BIRTHTIME = "st_birthtime";
        private const string MTIME = "st_mtime";

        protected DateTime getDateValue(string key)
        {
            var nanosec = long.Parse(GetFileInfo()[key]);
            var milisec = nanosec * 1e-6;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            DateTime unix = DateTime.UnixEpoch;

#else
            DateTime unix = DateTimeOffset.FromUnixTimeSeconds(0).UtcDateTime;
#endif
            return unix.AddMilliseconds(milisec);
        }

        public DateTime CreationTime => getDateValue(BIRTHTIME);

        public DateTime LastModifiedTime => getDateValue(MTIME);

        public AFCSessionBase Session { get; }

        public AFCItem(AFCSessionBase session, string path)
        {
            Path = path;
            Session = session;
            var infos = session.GetFileInfo(path);
        }

        public void Delete()
        {
            Session.Delete(Path);
        }

        virtual public bool Exist()
        {
            return GetFileInfo().Count > 0 && IsItemTypeSupported(ItemType);
        }

        public void MoveTo(string to)
        {
            Session.Move(this.Path, to);
            Path = to;
        }
        public void MakeLink(AFCLinkType linkType, string linkPath)
        {
            Session.MakeLink(this.Path, linkPath,linkType);
        }
    }
}
