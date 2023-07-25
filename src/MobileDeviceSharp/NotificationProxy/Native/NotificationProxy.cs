using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MobileDeviceSharp.CompilerServices;
using MobileDeviceSharp.Native;

namespace MobileDeviceSharp.NotificationProxy.Native
{
    internal static partial class NotificationProxy
    {

        public const string LibraryName = "imobiledevice";

        static NotificationProxy()
        {
            LibraryResolver.EnsureRegistered();
        }
    }
}
