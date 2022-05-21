using System.Runtime.InteropServices;

namespace IOSLib.NotificationProxy.Native
{
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void NotificationProxyNotifyCallBack([MarshalAsAttribute(UnmanagedType.LPStr)] string notification, System.IntPtr userData);
}
