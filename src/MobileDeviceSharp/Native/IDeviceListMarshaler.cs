using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileDeviceSharp.Native
{


    public class IDeviceListMarshaler : ArrayMarshaller<string, UTF8Marshaler>
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
