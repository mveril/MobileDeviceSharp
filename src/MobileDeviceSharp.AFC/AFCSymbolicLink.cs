using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.AFC
{
    /// <summary>
    /// Represents a symbolic link in the AFC file system.
    /// </summary>
    public sealed class AFCSymbolicLink : AFCItem
    {

        internal AFCSymbolicLink(AFCSessionBase session, string path) : base(session, path)
        {

        }

        /// <summary>
        /// Determines if the specified AFC item type is supported by this class.
        /// </summary>
        /// <param name="itemType">The AFC item type to check.</param>
        /// <returns>True if the item type is supported, otherwise false.</returns>
        protected override bool IsItemTypeSupported(AFCItemType itemType)
        {
            return itemType == AFCItemType.SymbolicLink;
        }

        /// <summary>
        /// Reads the target path of the symbolic link.
        /// </summary>
        /// <param name="recursive">Specifies whether to recursively resolve symbolic links.</param>
        /// <returns>The target path of the symbolic link.</returns>
        public string ReadLink(bool recursive)
        {
            var TargetPath = GetFileInfo()["LinkTarget"];
            if (recursive)
            {
                var target = new AFCSymbolicLink(Session, TargetPath);
                if (target.Exists)
                {
                    return target.ReadLink(true);
                }
            }
            return TargetPath;
        }

        /// <summary>
        /// Reads the target path of the symbolic link.
        /// </summary>
        /// <returns>The target path of the symbolic link.</returns>
        public string ReadLink()
        {
            return ReadLink(false);
        }
    }
}
