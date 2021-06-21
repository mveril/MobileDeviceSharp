using System.Runtime.InteropServices;

namespace IOSLib.Native
{
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void UsbmuxdEventCallBack(ref UsbmuxdEvent @event, System.IntPtr userData);
}