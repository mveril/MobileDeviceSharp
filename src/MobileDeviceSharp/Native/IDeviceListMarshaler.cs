using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileDeviceSharp.Native
{

    /// <summary>
    /// A marshaller for the <see cref="MobileDeviceSharp.Native.IDevice.idevice_get_device_list(out string[], out int)"/>
    /// </summary>
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
