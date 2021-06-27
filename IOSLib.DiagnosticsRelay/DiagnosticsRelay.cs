using System;
using System.Collections.Generic;
using System.Text;
using static IOSLib.DiagnosticsRelay.Native.DiagnosticsRelay;
using IOSLib.Native;
using IOSLib.DiagnosticsRelay.Native;

namespace IOSLib.DiagnosticsRelay
{
    public class DiagnosticsRelayService : IDisposable
    {
        public DiagnosticsRelayService(IDevice device)
        {
            var ex = diagnostics_relay_client_start_service(device.Handle, out var handle,null).GetException();
            if (ex != null)
            {
                throw ex;
            }
            Handle = handle;
        }

        public void Reboot(DiagnosticsRelayAction action)
        {
            var ex = diagnostics_relay_restart(Handle, action).GetException();
            if (ex != null)
            {
                throw ex;
            }
        }

        public void Shutdown(DiagnosticsRelayAction action)
        {
            var ex = diagnostics_relay_shutdown(Handle, action).GetException();
            if (ex != null)
            {
                throw ex;
            }
        }

        public DiagnosticsRelayClientHandle Handle { get; }

        public void Dispose()
        {
            ((IDisposable)Handle).Dispose();
        }
    }
}
