using MobileDeviceSharp.DiagnosticsRelay.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.DiagnosticsRelay
{
    /// <summary>
    /// Extension methods for the <see cref="IDevice"/> interface.
    /// </summary>
    public static class IDeviceExtention
    {
        /// <summary>
        /// Shuts down the specified device.
        /// </summary>
        /// <param name="device">The device to shut down.</param>
        public static void Shutdown(this IDevice device)
        {
            using (var relay = new DiagnosticsRelaySession(device))
            {
                relay.Shutdown(DiagnosticsRelayAction.ActionFlagDisplayFail);
            }
        }

        /// <summary>
        /// Reboots the specified device.
        /// </summary>
        /// <param name="device">The device to reboot.</param>
        public static void Reboot(this IDevice device)
        {
            using (var relay = new DiagnosticsRelaySession(device))
            {
                relay.Reboot(DiagnosticsRelayAction.ActionFlagDisplayFail);
            }
        }
    }
}
