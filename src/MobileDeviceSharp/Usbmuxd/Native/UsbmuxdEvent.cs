using System.Runtime.InteropServices;
#if NET7_0_OR_GREATER
using System.Runtime.InteropServices.Marshalling;
#endif

namespace MobileDeviceSharp.Usbmuxd.Native
{
#if NET7_0_OR_GREATER
    [NativeMarshalling(typeof(UsbmuxdEventMarshaller))]
#endif
    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct UsbmuxdEvent
    {

        public UsbmuxdEventType @event;

        public UsbmuxdDeviceInfo device;
    }
}
