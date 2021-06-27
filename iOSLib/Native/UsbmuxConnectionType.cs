﻿using System;

namespace IOSLib.Native
{
    /// <summary>
    /// Type of connection a device is available on 
    /// </summary>
    [Flags]
    public enum UsbmuxConnectionType : int
    {

        Usb = 1,

        Network = 2,

        All = Usb | Network,
        
    }
}