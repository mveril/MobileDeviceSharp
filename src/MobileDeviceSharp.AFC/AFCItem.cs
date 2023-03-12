using System;
using System.Collections.Generic;
using System.Text;
using static MobileDeviceSharp.AFC.Native.AFC;
using MobileDeviceSharp.AFC.Native;
using Mono.Unix;
using System.Linq;

namespace MobileDeviceSharp.AFC
{
    /// <summary>
    /// Represent an Apple FIle conduit item.
    /// </summary>
    public abstract class AFCItem
    {
        /// <summary>
        /// Gets the type of the AFC item.
        /// </summary>
        public AFCItemType ItemType => GetItemType();

        internal IReadOnlyDictionary<string, string> GetFileInfo()
        {
            return Session.GetFileInfo(Path);
        }

        internal AFCItemType GetItemType()
        {
            return Session.GetItemType(Path);
        }

        /// <summary>
        /// Determines if the specified AFC item type is supported.
        /// </summary>
        /// <param name="itemType">The AFC item type to check for support.</param>
        /// <returns>True if the specified AFC item type is supported, otherwise false.</returns>
        protected abstract bool IsItemTypeSupported(AFCItemType itemType);

        /// <summary>
        /// Gets the path of the AFC item.
        /// </summary>
        public string Path { get; protected set; }

        const string BIRTHTIME = "st_birthtime";
        private const string MTIME = "st_mtime";

        /// <summary>
        /// Get value for a date from a FileInfo key.
        /// </summary>
        /// <param name="key">The FileInfo key</param>
        /// <returns></returns>
        protected DateTime GetDateValue(string key)
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
        /// <summary>
        /// Gets the creation time of the item.
        /// </summary>
        public DateTime CreationTime => GetDateValue(BIRTHTIME);

        /// <summary>
        /// Gets the last modified time.
        /// </summary>
        public DateTime LastModifiedTime => GetDateValue(MTIME);

        /// <summary>
        /// Get the Apple File Conduit Session.
        /// </summary>
        public AFCSessionBase Session { get; }

        /// <summary>
        /// Gets an afc item from the specified <see cref="AFCSession"/> at the specified <paramref name="path"/>.
        /// </summary>
        public AFCItem(AFCSessionBase session, string path)
        {
            Path = path;
            Session = session;
        }

        /// <summary>
        /// Delete this item.
        /// </summary>
        public void Delete()
        {
            var hresult = afc_remove_path(Session.Handle, Path);
            if (hresult.IsError())
                throw hresult.GetException().ToStandardException(this);
        }

        /// <summary>
        /// Make a symbolic link at the specified <paramref name="path"/> for this item.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="linkType">The unix link type we need to use to create the link.</param>
        /// <returns></returns>
        public AFCItem MakeLink(string path, AFCLinkType linkType)
        {
            var hresult = afc_make_link(Session.Handle, linkType, Path, path);
            if (hresult.IsError())
                throw hresult.GetException().ToStandardException(this);
            return Session.Root.GetItem(path);
        }

        /// <summary>
        /// Check wether the specified item exist.
        /// </summary>
        public bool Exists => GetFileInfo().Count > 0 && IsItemTypeSupported(ItemType);

        /// <summary>
        /// Get the name of the item.
        /// </summary>
        public string Name => UnixPath.GetFileName(Path);

        /// <summary>
        /// Get the extension of the item.
        /// </summary>
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

        /// <summary>
        /// Move the file to a new location.
        /// </summary>
        /// <param name="destination">The path to move the item to, which can specify a different item name.</param>
        public void MoveTo(string destination)
        {
            var hresult = afc_rename_path(Session.Handle, Path, destination);
            if (hresult.IsError())
                throw hresult.GetException();
            Path = destination;
        }

        /// <summary>
        /// Create a symbolic link.
        /// </summary>
        /// <param name="linkType">The type of symbolic link.</param>
        /// <param name="linkPath">The path of the symbolic link.</param>
        /// <returns></returns>
        public AFCItem MakeLink(AFCLinkType linkType, string linkPath)
        {
            var hresult = afc_make_link(Session.Handle, linkType, Path, linkPath);
            if (hresult.IsError())
                throw hresult.GetException().ToStandardException(this);
            return Session.Root.GetItem(linkPath);
        }

        /// <summary>
        /// Gets an instance of the parent directory.
        /// </summary>
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
