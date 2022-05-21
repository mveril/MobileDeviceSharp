using System;
using System.Collections.Generic;
using System.Text;
using static IOSLib.AFC.Native.AFC;
using IOSLib.AFC.Native;
using Mono.Unix;
using System.Linq;

namespace IOSLib.AFC
{
    public abstract class AFCItem
    {

        public AFCItemType ItemType => GetItemType();

        internal IReadOnlyDictionary<string, string> GetFileInfo()
        {
            return Session.GetFileInfo(Path);
        }

        internal AFCItemType GetItemType()
        {
            return Session.GetItemType(Path);
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
        }

        public void Delete()
        {
            var hresult = afc_remove_path(Session.Handle, Path);
            if (hresult.IsError())
                throw hresult.GetException();
        }

        public AFCItem MakeLink(string path, AFCLinkType linkType)
        {
            var hresult = afc_make_link(Session.Handle, linkType, Path, path);
            if (hresult.IsError())
                throw hresult.GetException();
            return Session.Root.GetItem(path);
        }

        public bool Exists => GetFileInfo().Count > 0 && IsItemTypeSupported(ItemType);

        public string Name => UnixPath.GetFileName(Path);

        public string Extension
        {
            get
            {
                var index = Name.LastIndexOf('.');
                if (index != -1)
                {
                    return Name.Substring(index);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public void MoveTo(string to)
        {
            var hresult = afc_rename_path(Session.Handle, Path, to);
            if (hresult.IsError())
                throw hresult.GetException();
            Path = to;
        }
        public AFCItem MakeLink(AFCLinkType linkType, string linkPath)
        {
            var hresult = afc_make_link(Session.Handle, linkType, Path, linkPath);
            if (hresult.IsError())
                throw hresult.GetException();
            return Session.Root.GetItem(linkPath);
        }

        public AFCDirectory? Parent
        {
            get
            {
                var dir = UnixPath.GetDirectoryName(Path);
                if (dir == null)
                {
                    return null;
                }
                else
                {
                    return new AFCDirectory(Session, dir);
                }                
            }
        }
    }
}
