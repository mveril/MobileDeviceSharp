using System.Runtime.InteropServices;

namespace IOSLib.Native
{
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void NotificationProxyNotifyCallBack([MarshalAsAttribute(UnmanagedType.LPStr)] string notification, System.IntPtr userData);
}