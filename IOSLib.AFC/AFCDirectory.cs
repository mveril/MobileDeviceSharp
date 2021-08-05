using System;
using System.Collections.Generic;
using System.Text;
using IOSLib.AFC.Native;
using static IOSLib.AFC.Native.AFC;

namespace IOSLib.AFC
{
    public class AFCDirectory : AFCItem
    {
        internal AFCDirectory(AFCSessionBase session, string path) : base(session, path)
        {

        }

        protected override bool IsItemTypeSupported(AFCItemType itemType)
        {
            return itemType == AFCItemType.Directory;
        }

        public void Create()
        {
            var ex = afc_make_directory(Session.Handle, Path).GetException();
            if (ex != null)
                throw ex;
        }

        public AFCDirectory CreateSubDirectory(string name)
        {
            var sub = GetSubDirectory(Path);
            sub.Create();
            return sub;
        }

        public AFCDirectory GetSubDirectory(string name)
        {
            var subpath = $"{Path}/{name}";
            return new AFCDirectory(Session, subpath);
        }

        public AFCFile GetFile(string name)
        {
            var subpath = $"{Path}/{name}";
            return new AFCFile(Session, subpath);
        }

        public AFCDirectory CreateSubPath(string subpath)
        {
            var curr = this;
            foreach (var item in subpath.Split('/'))
            {
                curr = curr.CreateSubDirectory(item);
            }
            return curr;
        }

        public IEnumerable<AFCItem> EnumerateItems()
        {
            afc_read_directory(Session.Handle, Path, out var items);
            foreach (var item in items)
            {
                var path = $"{Path}/{item}";
                var type = Session.GetItemType(path);
                if (type == AFCItemType.File)
                {
                    yield return new AFCFile(Session, path);
                }
                else if (type == AFCItemType.Directory)
                {
                    yield return new AFCDirectory(Session, path);
                }
                else
                {
                    yield return new AFCItem(Session, path);
                }
            }
        }

        public void Delete(bool recursive)
        {
            this.Session.Delete(Path, recursive);
        }
    }
}
