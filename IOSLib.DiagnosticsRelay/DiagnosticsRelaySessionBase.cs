using System;
using System.Collections.Generic;
using System.Text;
using static IOSLib.DiagnosticsRelay.Native.DiagnosticsRelay;
using IOSLib.Native;
using IOSLib.DiagnosticsRelay.Native;

namespace IOSLib.DiagnosticsRelay
{
    public abstract class DiagnosticsRelaySessionBase : ServiceSessionBase<DiagnosticsRelayClientHandle,DiagnosticsRelayError> 
    {
        public DiagnosticsRelaySessionBase(IDevice device) : base(device, new StartServiceCallback<DiagnosticsRelayClientHandle, DiagnosticsRelayError>(diagnostics_relay_client_start_service))
        {

        }

        public DiagnosticsRelaySessionBase(IDevice device, string serviceID, bool withEscrowBag) : base(device,serviceID,withEscrowBag,new ClientNewCallback<DiagnosticsRelayClientHandle, DiagnosticsRelayError>(diagnostics_relay_client_new))
        {

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
    }
}
