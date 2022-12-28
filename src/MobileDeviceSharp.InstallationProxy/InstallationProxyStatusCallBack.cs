using System;
using System.Runtime.InteropServices;
using MobileDeviceSharp.PropertyList.Native;

namespace MobileDeviceSharp.InstallationProxy.Native
{
    public delegate void InstallationProxyStatusCallBack([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PlistHandleNotOwnedMarshaler))] PlistHandle command, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PlistHandleNotOwnedMarshaler))] PlistHandle status, IntPtr userData);
}
