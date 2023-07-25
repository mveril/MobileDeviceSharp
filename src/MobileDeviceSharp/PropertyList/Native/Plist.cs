using MobileDeviceSharp.CompilerServices;
using MobileDeviceSharp.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileDeviceSharp.PropertyList.Native
{
    internal static partial class Plist
    {
        public const string LibraryName = "plist";

        static Plist()
        {
            MobileDeviceSharp.Native.LibraryResolver.EnsureRegistered();
        }
    }
}
