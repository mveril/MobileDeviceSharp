using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IOSLib.Native
{
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void iDeviceEventCallBack(ref iDeviceEvent @event, System.IntPtr userData);

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct iDeviceEvent
    {

        public iDeviceEventType @event;

        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
        public string udid;

        public iDeviceConnectionType conn_type;

    }
}
