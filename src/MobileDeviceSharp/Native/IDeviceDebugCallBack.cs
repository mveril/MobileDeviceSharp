#if !NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileDeviceSharp.Native
{

    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void IDeviceDebugCallBack(IntPtr message);
}
#endif
