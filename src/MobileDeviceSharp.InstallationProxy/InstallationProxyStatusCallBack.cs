#if !NET7_0_OR_GREATER
using System;
using System.Runtime.InteropServices;
using MobileDeviceSharp.PropertyList.Native;

namespace MobileDeviceSharp.InstallationProxy.Native
{
    public delegate void InstallationProxyStatusCallBack(PlistNotOwnedHandle command, PlistNotOwnedHandle status, IntPtr userData);
}
#endif
