using System.Runtime.InteropServices;

namespace IOSLib.Native
{
    [StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct UsbmuxdEvent
    {

        public int @event;

        public UsbmuxdDeviceInfo device;
    }
}