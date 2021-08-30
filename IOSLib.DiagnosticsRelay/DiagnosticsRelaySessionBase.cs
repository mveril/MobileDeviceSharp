using System;
using System.Collections.Generic;
using System.Text;
using static IOSLib.DiagnosticsRelay.Native.DiagnosticsRelay;
using IOSLib.Native;
using IOSLib.DiagnosticsRelay.Native;
using IOSLib.PropertyList.Native;
using IOSLib.PropertyList;

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

        public void Sleep()
        {
            diagnostics_relay_sleep(Handle);
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
        public PlistNode RequestDiagnostics(string type)
        {
            var ex = diagnostics_relay_request_diagnostics(Handle, type, out var plistHandle).GetException();
            if (ex != null)
            {
                throw ex;
            }
            return PlistNode.From(plistHandle);
        }

        public PlistNode QueryMobilegestalt(string key)
        {
            Exception ex;
            PlistHandle result;
            using (var keynode = new PlistKey(key))
            {
                ex = diagnostics_relay_query_mobilegestalt(Handle, keynode.Handle, out result).GetException();
            }
            if (ex != null)
            {
                throw ex;
            }
            return PlistNode.From(result);
        }

        public PlistNode QueryIoregistryEntry(string entryName,string entryClass)
        {
            var ex = diagnostics_relay_query_ioregistry_entry(Handle, entryName, entryClass, out var result).GetException();
            if (ex != null)
            {
                throw ex;
            }
            return PlistNode.From(result);
        }

        public PlistNode QueryIoregistryPlane(string plane)
        {
            var ex = diagnostics_relay_query_ioregistry_plane(Handle, plane, out var result).GetException();
            if (ex != null)
            {
                throw ex;
            }
            return PlistNode.From(result);
        }
    }
}
