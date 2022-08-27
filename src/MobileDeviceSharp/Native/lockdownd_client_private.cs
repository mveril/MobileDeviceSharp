using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileDeviceSharp.Native
{
#pragma warning disable IDE1006 // Naming styles (because this struct is used for PInvoke)
    [StructLayout(LayoutKind.Sequential)]
    internal struct lockdownd_client_private
    {
        internal IntPtr parent;

        internal int ssl_enabled;
        internal IntPtr session_id;
        internal IntPtr udid;
        internal IntPtr label;
        internal uint mux_id;
    }
#pragma warning restore IDE1006 // Naming styles (because this struct is used for PInvoke)
}
