using IOSLib.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IOSLib.Native
{
    class IDeviceListExtendedMarshaler : IDeviceListMarshaler
    {
        private static readonly Lazy<IDeviceListExtendedMarshaler> s_static_instance = new();

        public override void CleanUpNativeData(IntPtr pNativeData)
        {
            IDevice.idevice_device_list_extended_free(pNativeData);
        }

        public static new IDeviceListExtendedMarshaler GetInstance()
        {
            return s_static_instance.Value;
        }

        public static new ICustomMarshaler GetInstance(string cookie)
        {
            return s_static_instance.Value;
        }
    }
}
