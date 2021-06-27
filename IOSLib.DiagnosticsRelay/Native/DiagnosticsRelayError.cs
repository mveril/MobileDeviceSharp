namespace IOSLib.DiagnosticsRelay.Native
{
    /// <summary>
    /// Error Codes 
    /// </summary>
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