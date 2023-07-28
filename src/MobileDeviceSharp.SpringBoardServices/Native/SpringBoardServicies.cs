using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MobileDeviceSharp.CompilerServices;
using MobileDeviceSharp.Native;
using MobileDeviceSharp.PropertyList.Native;

namespace MobileDeviceSharp.SpringBoardServices.Native
{
    internal static partial class SpringBoardServices
    {

        public const string LibraryName = "imobiledevice";

        static SpringBoardServices()
        {
            LibraryResolver.EnsureRegistered();
        }

    }
}
