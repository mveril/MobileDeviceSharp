using System.Runtime.InteropServices;

namespace MobileDeviceSharp.Usbmuxd.Native
{
    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct UsbmuxdEvent
    {

        public UsbmuxdEventType @event;

        public UsbmuxdDeviceInfo device;
    }
}
