using MobileDeviceSharp.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileDeviceSharp.AFC.Native
{
    internal static partial class AFC
    {
        public const string LibraryName = "imobiledevice";

        static AFC()
        {
            LibraryResolver.EnsureRegistered();
        }
    }
}
