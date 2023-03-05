namespace MobileDeviceSharp.Usbmuxd.Native
{
    /// <summary>
    /// Event types for event callback function
    /// </summary>
    public enum UsbmuxdEventType : int
    {

        DeviceAdd = 1,

        DeviceRemove = 2,

        DevicePaired = 3,
    }
}
