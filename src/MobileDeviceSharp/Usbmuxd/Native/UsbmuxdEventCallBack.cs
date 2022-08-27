using System.Runtime.InteropServices;

namespace MobileDeviceSharp.Usbmuxd.Native
{
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    internal delegate void UsbmuxdEventCallBack(ref UsbmuxdEvent @event, System.IntPtr userData);
}
