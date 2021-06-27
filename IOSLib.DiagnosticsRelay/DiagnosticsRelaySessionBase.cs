using System;
using System.Collections.Generic;
using System.Text;
using static IOSLib.DiagnosticsRelay.Native.DiagnosticsRelay;
using IOSLib.Native;
using IOSLib.DiagnosticsRelay.Native;

namespace IOSLib.DiagnosticsRelay
{
    public abstract class DiagnosticsRelaySessionBase : ServiceSessionBase<DiagnosticsRelayClientHandle> 
    {
        public DiagnosticsRelaySessionBase(IDevice device) : base(device)
        {

        }

        public DiagnosticsRelaySessionBase(IDevice device, string serviceID) : base(device,serviceID)
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

        protected override DiagnosticsRelayClientHandle Init(LockdownServiceDescriptorHandle Descriptor)
        {
            var ex = diagnostics_relay_client_new(Device.Handle,Descriptor, out var handle).GetException();
            if (ex != null)
            {
                throw ex;
            }
            return handle;
        }

        protected override DiagnosticsRelayClientHandle Init()
        {
            var ex = diagnostics_relay_client_start_service(Device.Handle, out var handle, null).GetException();
            if (ex != null)
            {
                throw ex;
            }
            return handle;
        }
    }
}
