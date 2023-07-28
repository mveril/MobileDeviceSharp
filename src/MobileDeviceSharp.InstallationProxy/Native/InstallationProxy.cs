using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.CompilerServices;
using System.Runtime.InteropServices;
using MobileDeviceSharp.PropertyList;
using MobileDeviceSharp.PropertyList.Native;
using MobileDeviceSharp.InstallationProxy;
using MobileDeviceSharp.Native;

namespace MobileDeviceSharp.InstallationProxy.Native
{
    internal static partial class InstallationProxy
    {
        public const string LibraryName = "imobiledevice";

        static InstallationProxy()
        {
            LibraryResolver.EnsureRegistered();
        }
    }
}
