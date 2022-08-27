namespace MobileDeviceSharp
{
    /// <summary>
    /// Represent the type of device IPhone, iPad...
    /// </summary>
    public enum DeviceClass : int
    {
        /// <summary>
        /// The device type is unknow
        /// </summary>
        Unknow = -1,
        /// <summary>
        /// Represent an iPhone
        /// </summary>
        iPhone,
        /// <summary>
        /// Represent an iPad
        /// </summary>
        iPad,
        /// <summary>
        /// Represent an iPod
        /// </summary>
        iPod,
        /// <summary>
        /// Represent an Apple Watch
        /// </summary>
        Watch,
        /// <summary>
        /// Represent an AppleTV
        /// </summary>
        AppleTV
    }
}
