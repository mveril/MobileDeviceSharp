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

        private static readonly StartServiceCallback<DiagnosticsRelayClientHandle, DiagnosticsRelayError> s_startCallback = diagnostics_relay_client_start_service;
        
        private static readonly ClientNewCallback<DiagnosticsRelayClientHandle, DiagnosticsRelayError> s_clientNewCallback = diagnostics_relay_client_new;

        public DiagnosticsRelaySessionBase(IDevice device) : base(device, s_startCallback)
        {

        }

        public DiagnosticsRelaySessionBase(IDevice device, string serviceID, bool withEscrowBag) : base(device,serviceID,withEscrowBag,s_clientNewCallback)
        {

        }

        public void Sleep()
        {
            diagnostics_relay_sleep(Handle);
        }

        public void Reboot(DiagnosticsRelayAction action)
        {
            var hresult = diagnostics_relay_restart(Handle, action);
            if (hresult.IsError())
                throw hresult.GetException();
        }

        public void Shutdown(DiagnosticsRelayAction action)
        {
            var hresult = diagnostics_relay_shutdown(Handle, action);
            if (hresult.IsError())
                throw hresult.GetException();
        }
        public PlistNode RequestDiagnostics(string type)
        {
            var hresult = diagnostics_relay_request_diagnostics(Handle, type, out var plistHandle);
            if (hresult.IsError())
                throw hresult.GetException();
            return PlistNode.From(plistHandle);
        }

        public PlistNode QueryMobilegestalt(string key)
        {
            DiagnosticsRelayError hresult;
            PlistHandle result;
            using (var keynode = new PlistKey(key))
            {
                hresult = diagnostics_relay_query_mobilegestalt(Handle, keynode.Handle, out result);
            }
            if (hresult.IsError())
                throw hresult.GetException();
            return PlistNode.From(result);
        }

        public PlistNode QueryIoregistryEntry(string entryName,string entryClass)
        {
            var hresult = diagnostics_relay_query_ioregistry_entry(Handle, entryName, entryClass, out var result);
            if (hresult.IsError())
                throw hresult.GetException();
            return PlistNode.From(result);
        }

        public PlistNode QueryIoregistryPlane(string plane)
        {
            var hresult = diagnostics_relay_query_ioregistry_plane(Handle, plane, out var result);
            if (hresult.IsError())
                throw hresult.GetException();
            return PlistNode.From(result);
        }
    }
}
