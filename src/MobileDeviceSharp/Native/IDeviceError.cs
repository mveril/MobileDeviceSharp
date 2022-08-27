using MobileDeviceSharp;

namespace MobileDeviceSharp.Native
{
    /// <summary>
    /// Error Codes
    /// </summary>
    [Exception(typeof(IDeviceException))]
    public enum IDeviceError : int
    {

        Success = 0,

        InvalidArg = -1,

        UnknownError = -2,

        NoDevice = -3,

        NotEnoughData = -4,

        SslError = -6,

        Timeout = -7,
    }
}
