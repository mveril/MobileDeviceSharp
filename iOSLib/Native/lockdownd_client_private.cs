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
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
        internal string session_id;
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
        internal string udid;
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
        internal string label;
        internal UInt32 mux_id;
    }
}
