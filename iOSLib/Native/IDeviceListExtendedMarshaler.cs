using IOSLib.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IOSLib.Native
{
    class IDeviceListExtendedMarshaler : IDeviceListMarshaler
    {
        static Lazy<IDeviceListExtendedMarshaler> static_instance = new Lazy<IDeviceListExtendedMarshaler>();

        public override void CleanUpNativeData(IntPtr pNativeData)
        {
            IDevice.idevice_device_list_extended_free(pNativeData);
        }

        public static new IDeviceListExtendedMarshaler GetInstance()
        {
            return static_instance.Value;
        }

        public static new IDeviceListExtendedMarshaler GetInstance(string cookie)
        {
            return static_instance.Value;
        }
    }
}
