using System;

namespace IOSLib.Usbmuxd.Native
{
    /// <summary>
    /// Type of connection a device is available on 
    /// </summary>
    [Flags]
    public enum IDeviceLookupOptions : int
    {

        Usb = 1,

        Network = 2,

        All = Usb | Network,

        PreferNetwork,

        All_PreferNetwork = All | PreferNetwork,

    }
}
