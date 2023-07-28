using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.AFC.Native;
using MobileDeviceSharp.CompilerServices;
using MobileDeviceSharp.Native;
using MobileDeviceSharp.PropertyList.Native;

namespace MobileDeviceSharp.HouseArrest.Native
{
    internal static partial class HouseArrest
    {

        public const string LibraryName = "imobiledevice";

        static HouseArrest()
        {
            LibraryResolver.EnsureRegistered();
        }
    }
}
