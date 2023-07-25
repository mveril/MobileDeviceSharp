#if !NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileDeviceSharp.Native
{
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void IDeviceEventCallBack(ref IDeviceEvent @event, System.IntPtr userData);

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct IDeviceEvent
    {

        public IDeviceEventType @event;

        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
        public string udid;

        public IDeviceConnectionType conn_type;

    }
}
#endif
