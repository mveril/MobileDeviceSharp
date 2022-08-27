namespace MobileDeviceSharp.DiagnosticsRelay.Native
{
    /// <summary>
    /// Error Codes
    /// </summary>
    [Exception(typeof(DiagnosticsRelayException))]
    public enum DiagnosticsRelayError : int
    {

        Success = 0,

        InvalidArg = -1,

        PlistError = -2,

        MuxError = -3,

        UnknownRequest = -4,

        UnknownError = -256,
    }
}
