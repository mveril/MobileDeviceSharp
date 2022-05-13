using System.Runtime.InteropServices;

namespace IOSLib.Native
{
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct UsbmuxdDeviceInfo
    {

        public uint handle;

        public uint product_id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 44)]
        public string udid;

        public UsbmuxConnectionType conn_type;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 200)]
        public string conn_data;
    }
}
