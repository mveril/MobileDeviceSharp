namespace MobileDeviceSharp.DiagnosticsRelay.Native
{
    public enum DiagnosticsRelayAction : int
    {
        /// <summary>
        /// Wait until diagnostics disconnects before execution.
        /// </summary>
        ActionFlagWaitForDisconnect = 2,
        /// <summary>
        /// show a "FAIL" dialog.
        /// </summary>
        ActionFlagDisplayPass = 4,
        /// <summary>
        /// Show an "OK" dialog.
        /// </summary>
        ActionFlagDisplayFail = 8,
    }
}
