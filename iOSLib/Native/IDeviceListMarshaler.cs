using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IOSLib.Native
{


    class IDeviceListMarshaler : UTF8ArrayMarshaler
    {

        static Lazy<IDeviceListMarshaler> static_instance = new Lazy<IDeviceListMarshaler>();
        
        public override void CleanUpNativeData(IntPtr pNativeData)
        {
            IDevice.idevice_device_list_free(pNativeData);
        }

        public static new IDeviceListMarshaler GetInstance()
        {
            return static_instance.Value;
        }

        public static new IDeviceListMarshaler GetInstance(string cookie)
        {
            return static_instance.Value;
        }
    }
}
