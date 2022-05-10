using System.Runtime.InteropServices;

namespace IOSLib.Native
{
    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct UsbmuxdEvent
    {

        public UsbmuxdEventType @event;

        public UsbmuxdDeviceInfo device;
    }
}
