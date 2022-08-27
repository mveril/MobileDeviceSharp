using System.Runtime.InteropServices;

namespace MobileDeviceSharp.NotificationProxy.Native
{
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void NotificationProxyNotifyCallBack([MarshalAsAttribute(UnmanagedType.LPStr)] string notification, System.IntPtr userData);
}
