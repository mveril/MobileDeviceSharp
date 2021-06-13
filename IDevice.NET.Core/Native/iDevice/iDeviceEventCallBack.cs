using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IDevice.NET.Core.Native.iDevice
{
    [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate void iDeviceEventCallBack(ref iDeviceEvent @event, System.IntPtr userData);

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct iDeviceEvent
    {

        public iDeviceEventType @event;

        public System.IntPtr udid;

        public iDeviceConnectionType conn_type;

        public string udidString
        {
            get
            {
                return Marshal.PtrToStringUTF8(this.udid);
            }
        }
    }
}
