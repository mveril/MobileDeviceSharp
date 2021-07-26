using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IOSLib.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct lockdownd_client_private
    {
        internal IntPtr parent;
        internal int ssl_enabled;
        internal IntPtr session_id;
        internal IntPtr udid;
        internal IntPtr label;
        internal UInt32 mux_id;
    }
}
