using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MobileDeviceSharp.AFC.Native;

namespace MobileDeviceSharp.AFC
{
    public partial class AFCException
    {
        public Exception ToStandardException(AFCItem item)
        {
            return ToStandardException(item.ItemType, item.Path);
        }

        public Exception ToStandardException(AFCItemType itemType)
        {
            return ToStandardException(itemType, null);
        }

        public Exception ToStandardException(AFCItemType itemType, string? path)
        {
            var pathString = string.IsNullOrEmpty(path) ? "" : $" : {path}";
            return (AFCError)ErrorCode switch
            {
                AFCError.ObjectNotFound => new UnauthorizedAccessException($"{itemType.Name} not found{pathString}.", this),
                AFCError.ObjectIsDir => new UnauthorizedAccessException($"Object is directory{pathString}.", this),
                AFCError.PermDenied => new UnauthorizedAccessException($"Permission denied{pathString}.", this),
                AFCError.ObjectExists => new IOException($"{itemType.Name} already exist{pathString}.", this),
                AFCError.IoError or AFCError.WriteError or AFCError.ReadError or AFCError.DirNotEmpty or AFCError.NoMem or AFCError.NoResources or AFCError.NoSpaceLeft or AFCError.ObjectExists => new IOException($"IO error{pathString}.", this),
                _ => this,
            };
        }
    }
}
