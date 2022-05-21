using System.Runtime.InteropServices;

namespace IOSLib.Usbmuxd.Native
{
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    internal delegate void UsbmuxdEventCallBack(ref UsbmuxdEvent @event, System.IntPtr userData);
}
