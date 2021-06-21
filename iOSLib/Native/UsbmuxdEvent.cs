using System.Runtime.InteropServices;

namespace IOSLib.Native
{
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct UsbmuxdEvent
    {

        public int @event;

        public UsbmuxdDeviceInfo device;
    }
}