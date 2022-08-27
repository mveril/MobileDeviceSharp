using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileDeviceSharp.Native
{


    class IDeviceListMarshaler : UTF8ArrayMarshaler
    {

        private  static readonly Lazy<IDeviceListMarshaler> s_static_instance = new();

        public override void CleanUpNativeData(IntPtr pNativeData)
        {
            IDevice.idevice_device_list_free(pNativeData);
        }

        public static new IDeviceListMarshaler GetInstance()
        {
            return s_static_instance.Value;
        }

        public static new ICustomMarshaler GetInstance(string cookie)
        {
            return s_static_instance.Value;
        }
    }
}
