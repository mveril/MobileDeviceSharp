namespace MobileDeviceSharp.Native
{
    internal static partial class IDevice
    {
        public const string LibraryName = "imobiledevice";

        static IDevice()
        {
            LibraryResolver.EnsureRegistered();
        }
    }
}
