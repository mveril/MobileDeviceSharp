using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.CompilerServices;
using System.Runtime.InteropServices;
using MobileDeviceSharp.PropertyList;
using MobileDeviceSharp.PropertyList.Native;

namespace MobileDeviceSharp.Native
{
    internal static partial class Lockdown
    {
        public const string LibraryName = "imobiledevice";

        static Lockdown()
        {
            LibraryResolver.EnsureRegistered();
        }
    }
}
