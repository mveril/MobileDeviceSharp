using System;
using System.Collections.Generic;
using System.Text;
using static MobileDeviceSharp.AFC.Native.AFC;

namespace MobileDeviceSharp.AFC
{
    /// <summary>
    /// Represent information for the device drive accessible from the Apple File Conduit API.
    /// </summary>
    public class AFCDriveInfo
    {
        /// <summary>
        /// Initialize a new instance of a <see cref="DriveInfo"/> from the specified <paramref name="session"/>.
        /// </summary>
        /// <param name="session"></param>
        internal AFCDriveInfo(AFCSessionBase session)
        {
            Session = session;
        }

        private string GetInfo(string key)
        {
            afc_get_device_info_key(Session.Handle, key, out var value);
            return value;
        }

        /// <summary>
        /// Get the session used to create this <see cref="AFCDriveInfo"/>.
        /// </summary>
        public AFCSessionBase Session { get; }

        /// <summary>
        /// Get the amont of free space on the device.
        /// </summary>
        public long FreeSpace => long.Parse(GetInfo("FSFreeBytes"));

        /// <summary>
        /// Get the total space of the drive.
        /// </summary>
        public long TotalSpace => long.Parse(GetInfo("FSTotalBytes"));

        /// <summary>
        /// Get the block size of the drive.
        /// </summary>
        public int BlockSize => int.Parse(GetInfo("FSBlockSize"));
    }
}
