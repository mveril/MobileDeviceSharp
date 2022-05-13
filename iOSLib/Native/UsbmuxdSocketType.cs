using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib.Native
{
    public enum UsbmuxdSocketType
    {
        // Use UNIX sockets. The default on Linux and macOS.
        UNIX = 1,

     	// Use TCP sockets. The default and only option on Windows.
	    TCP = 2
    }
}
