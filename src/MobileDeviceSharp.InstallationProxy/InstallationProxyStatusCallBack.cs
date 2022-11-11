using System;
using MobileDeviceSharp.PropertyList.Native;

namespace MobileDeviceSharp.InstallationProxy.Native
{
    public delegate void InstallationProxyStatusCallBack(PlistHandle command, PlistHandle status, IntPtr userData);
}
