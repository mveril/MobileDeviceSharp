using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IDevice.NET.Core.Native.iDevice
{

    [UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate void iDeviceDebugCallBack(IntPtr message);
}
