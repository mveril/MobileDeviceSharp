namespace IDevice.NET.Core.Native.iDevice
{
    /// <summary>
    /// Error Codes 
    /// </summary>
    public enum iDeviceError : int
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