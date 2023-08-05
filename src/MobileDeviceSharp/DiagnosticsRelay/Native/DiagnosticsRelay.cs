using MobileDeviceSharp.Native;
using MobileDeviceSharp.PropertyList.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.DiagnosticsRelay.Native
{
    internal static partial class DiagnosticsRelay
    {
        public const string LibraryName = "imobiledevice";

        static DiagnosticsRelay()
        {
            LibraryResolver.EnsureRegistered();
        }
    }
}
