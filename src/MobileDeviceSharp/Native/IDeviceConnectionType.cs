namespace MobileDeviceSharp.Native
{
    /// <summary>
    /// Type of connection a device is available on
    /// </summary>
    public enum IDeviceConnectionType : int
    {

        /// <summary>
        /// Represent an USB connection.
        /// </summary>
        Usbmuxd = 1,

        /// <summary>
        /// Represent a network connection.
        /// </summary>
        Network = 2,
    }
}
