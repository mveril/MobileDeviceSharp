using System.Runtime.InteropServices;
#if NET7_0_OR_GREATER
using System.Runtime.InteropServices.Marshalling;
#endif

namespace MobileDeviceSharp.Usbmuxd.Native
{
#if NET7_0_OR_GREATER
    [NativeMarshalling(typeof(UsbmuxdDeviceInfoMarshaller))]
#endif
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct UsbmuxdDeviceInfo
    {

        public uint handle;

        public uint product_id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 44)]
        public string udid;

        public IDeviceLookupOptions conn_type;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public byte[] conn_data;
    }
}
