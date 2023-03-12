using System;

namespace MobileDeviceSharp.Usbmuxd.Native
{
    /// <summary>
    /// Type of connection a device is available on
    /// </summary>
    [Flags]
    public enum IDeviceLookupOptions : int
    {
        /// <summary>
        /// Lookup USB devices.
        /// </summary>
        Usb = 1,

        /// <summary>
        /// Lookup network devices.
        /// </summary>
        Network = 2,

        /// <summary>
        /// Lookup all devices (<see cref="Usb"/> and <see cref="Network"/>).
        /// </summary>
        All = Usb | Network,

        PreferNetwork,

        /// <summary>
        /// Lookup all devices (<see cref="Usb"/> and <see cref="Network"/>) but prefer network devices.
        /// </summary>
        All_PreferNetwork = All | PreferNetwork,

    }
}
