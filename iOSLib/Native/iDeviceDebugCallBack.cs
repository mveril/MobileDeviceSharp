using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IOSLib.Native
{

    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void iDeviceDebugCallBack(IntPtr message);
}
