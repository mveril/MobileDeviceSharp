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

        private static readonly StartServiceCallback<DiagnosticsRelayClientHandle, DiagnosticsRelayError> startCallback = diagnostics_relay_client_start_service;
        
        private static readonly ClientNewCallback<DiagnosticsRelayClientHandle, DiagnosticsRelayError> clientNewCallback = diagnostics_relay_client_new;

        public DiagnosticsRelaySessionBase(IDevice device) : base(device, startCallback)
        {

        }

        public DiagnosticsRelaySessionBase(IDevice device, string serviceID, bool withEscrowBag) : base(device,serviceID,withEscrowBag,clientNewCallback)
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
