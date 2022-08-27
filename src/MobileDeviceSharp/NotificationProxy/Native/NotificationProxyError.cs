namespace MobileDeviceSharp.NotificationProxy.Native
{
    /// <summary>
    /// Error Codes
    /// </summary>
    [Exception(typeof(NotificationProxyException))]
    public enum NotificationProxyError : int
    {

        Success = 0,

        InvalidArg = -1,

        PlistError = -2,

        ConnFailed = -3,

        UnknownError = -256,
    }
}
