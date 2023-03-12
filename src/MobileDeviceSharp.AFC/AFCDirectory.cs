using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileDeviceSharp.AFC.Native;
using Mono.Unix;
using static MobileDeviceSharp.AFC.Native.AFC;

namespace MobileDeviceSharp.AFC
{
    /// <summary>
    /// Represent an Apple File conduit directory.
    /// </summary>
    public sealed class AFCDirectory : AFCItem
    {
        internal AFCDirectory(AFCSessionBase session, string path) : base(session, path)
        {

        }

        /// <inheritdoc/>
        protected override bool IsItemTypeSupported(AFCItemType itemType)
        {
            return itemType == AFCItemType.Directory;
        }

        /// <summary>
        /// Create this directory.
        /// </summary>
        public void Create()
        {
            var hresult = afc_make_directory(Session.Handle, Path);
            if (hresult.IsError())
                throw hresult.GetException();
        }

        /// <summary>
        /// Create subdirectory
        /// </summary>
        /// <param name="name">The name of the subdirectory.</param>
        /// <returns>The <see cref="AFCDirectory"/> representing the subdirectory. </returns>
        public AFCDirectory CreateSubDirectory(string name)
        {
            var sub = GetSubDirectory(UnixPath.Combine(Path ,name));
            sub.Create();
            return sub;
        }

        /// <summary>
        /// Get AFC child item by name.
        /// </summary>
        /// <param name="name">The nam of the item</param>
        /// <returns>The returned item.</returns>
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

        /// <summary>
        /// Get a subdirectory by name
        /// </summary>
        /// <param name="name">The name of the subdirectory</param>
        /// <returns>An instance of <see cref="AFCDirectory"/> representing the subdirectory.</returns>
        public AFCDirectory GetSubDirectory(string name)
        {
            var subpath = UnixPath.Combine(Path,name);
            return new AFCDirectory(Session, subpath);
        }

        /// <summary>
        /// Get a child file by name.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <returns>An instance of <see cref="AFCFile"/> representing the file.</returns>
        public AFCFile GetFile(string name)
        {
            var subpath = UnixPath.Combine(Path, name);
            return new AFCFile(Session, subpath);
        }

        /// <summary>
        /// Get a child symbolic link by name.
        /// </summary>
        /// <param name="name">The name of the symbolic link.</param>
        /// <returns>An instance of <see cref="AFCSymbolicLink"/> representing the symbilic link.</returns>
        public AFCSymbolicLink GetSymbolicLink(string name)
        {
            var subpath = UnixPath.Combine(Path, name);
            return new AFCSymbolicLink(Session, subpath);
        }

        /// <summary>
        /// Create arborescence of child directories.
        /// </summary>
        /// <param name="subpath">The relative path from this directory.</param>
        /// <returns>An instance of <see cref="AFCDirectory"/> representing the leaf directory.</returns>
        public AFCDirectory CreateSubPath(string subpath)
        {
            var curr = this;
            foreach (var item in subpath.Split(UnixPath.DirectorySeparatorChar))
            {
                curr = curr.CreateSubDirectory(item);
            }
            return curr;
        }

        /// <summary>
        /// Get all subitem of a directory.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{AFCItem}"/> containing all the subitems.</returns>
        public IEnumerable<AFCItem> GetItems()
        {
            var hresult = afc_read_directory(Session.Handle, Path, out var items);
            if (hresult.IsError())
                throw hresult.GetException().ToStandardException(this);
            return items.Except(new string[] { ".", ".." }).Select(item=> GetItem(item));
        }

        /// <summary>
        /// Delete the directory.
        /// </summary>
        /// <param name="recursive">A <see cref="bool"/> value used to specify if the directory can be deleted even if it's empty.</param>
        public void Delete(bool recursive)
        {
            if (recursive)
            {
                var hresult = afc_remove_path_and_contents(Session.Handle, Path);
                if (hresult.IsError())
                    throw hresult.GetException().ToStandardException(this);
            }
            else
            {
                Delete();
            }
        }
    }
}
