namespace MobileDeviceSharp.Native
{
    /// <summary>
    /// The event type for device add or removal
    /// </summary>
    public enum IDeviceEventType : int
    {
        /// <summary>
        /// A device is added.
        /// </summary>
        DeviceAdd = 1,

        /// <summary>
        /// A device is removed.
        /// </summary>
        DeviceRemove = 2,

        /// <summary>
        /// A device is paired.
        /// </summary>
        DevicePaired = 3,
    }
}
