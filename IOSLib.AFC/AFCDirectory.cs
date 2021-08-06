using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOSLib.AFC.Native;
using Mono.Unix;
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

        public AFCItem GetItem(string name)
        {
            var path = UnixPath.Combine(Path, name);
            var type = Session.GetItemType(path);
            if (type == AFCItemType.File)
            {
                return new AFCFile(Session, path);
            }
            else if (type == AFCItemType.Directory)
            {
                return new AFCDirectory(Session, path);
            }
            else if (type == AFCItemType.SymbolicLink)
            {
                return new AFCSymbolicLink(Session, path);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public AFCDirectory GetSubDirectory(string name)
        {
            var subpath = UnixPath.Combine(Path,name);
            return new AFCDirectory(Session, subpath);
        }

        public AFCFile GetFile(string name)
        {
            var subpath = UnixPath.Combine(Path, name);
            return new AFCFile(Session, subpath);
        }

        public AFCItem GetSymbolicLink(string name)
        {
            var subpath = UnixPath.Combine(Path, name);
            return new AFCSymbolicLink(Session, subpath);
        }

        public AFCDirectory CreateSubPath(string subpath)
        {
            var curr = this;
            foreach (var item in subpath.Split(UnixPath.DirectorySeparatorChar))
            {
                curr = curr.CreateSubDirectory(item);
            }
            return curr;
        }

        public IEnumerable<AFCItem> GetItems()
        {
            afc_read_directory(Session.Handle, Path, out var items);
            foreach (var item in items.Except(new string[] { ".", ".." }))
            {
                var path = UnixPath.Combine(Path,item);
                yield return GetItem(item);
            }
        }

        public void Delete(bool recursive)
        {
            this.Session.Delete(Path, recursive);
        }
    }
}
