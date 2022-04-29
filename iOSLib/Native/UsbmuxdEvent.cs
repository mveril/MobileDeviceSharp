using System.Runtime.InteropServices;

namespace IOSLib.Native
{
    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct UsbmuxdEvent
    {

        public int @event;

        public UsbmuxdDeviceInfo device;
    }
}
