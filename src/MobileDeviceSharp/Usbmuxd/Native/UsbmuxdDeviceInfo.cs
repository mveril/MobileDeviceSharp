using System.Runtime.InteropServices;

namespace MobileDeviceSharp.Usbmuxd.Native
{
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct UsbmuxdDeviceInfo
    {

        public uint handle;

        public uint product_id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 44)]
        public string udid;

        public IDeviceLookupOptions conn_type;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 200)]
        public string conn_data;
    }
}
