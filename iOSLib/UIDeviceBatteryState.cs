namespace IOSLib
{
    /// <summary>
    /// Represent the state of the device battery (see the <see href="https://docs.microsoft.com/dotnet/api/uikit.uidevicebatterystate">UIDeviceBatteryState</see> from the XAMARIN docs).
    /// </summary>
	public enum UIDeviceBatteryState
	{
        /// <summary>
        /// Can not determine the state of the battery.
        /// </summary>
		Unknown,
        /// <summary>
        /// The device is unplugged.
        /// </summary>
		Unplugged,

        /// <summary>
        /// The device's battery is currently charging.
        /// </summary>
		Charging,

        /// <summary>
        /// The device's battery is at full capacity.
        /// </summary>
		Full,
	}
}
