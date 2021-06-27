using IOSLib.DiagnosticsRelay.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib.DiagnosticsRelay
{
    public static class IDeviceExtention
    {

        public static void Shutdown(this IDevice device)
        {
            using (var relay = new DiagnosticsRelayService(device))
            {
                relay.Shutdown(DiagnosticsRelayAction.ActionFlagDisplayFail);
            }
        }
        public static void Reboot(this IDevice device)
        {
            using (var relay = new DiagnosticsRelayService(device))
            {
                relay.Reboot(DiagnosticsRelayAction.ActionFlagDisplayFail);
            }
        }
    }
}
