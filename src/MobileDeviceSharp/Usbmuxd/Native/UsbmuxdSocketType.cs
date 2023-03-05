using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.Usbmuxd.Native
{
    public enum UsbmuxdSocketType
    {
        /// <summary>
        /// Use UNIX sockets. The default on Linux and macOS.
        /// </summary>
        UNIX = 1,

        /// <summary>
        /// Use TCP sockets. The default and only option on Windows.
        /// </summary>
        TCP = 2
    }
}
