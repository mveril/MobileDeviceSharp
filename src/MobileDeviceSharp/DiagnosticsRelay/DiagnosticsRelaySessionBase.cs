﻿using System;
using System.Collections.Generic;
using System.Text;
using static MobileDeviceSharp.DiagnosticsRelay.Native.DiagnosticsRelay;
using MobileDeviceSharp.Native;
using MobileDeviceSharp.DiagnosticsRelay.Native;
using MobileDeviceSharp.PropertyList.Native;
using MobileDeviceSharp.PropertyList;
using System.Linq;

namespace MobileDeviceSharp.DiagnosticsRelay
{
    public abstract class DiagnosticsRelaySessionBase : ServiceSessionBase<DiagnosticsRelayClientHandle,DiagnosticsRelayError>
    {

        private static readonly StartServiceCallback<DiagnosticsRelayClientHandle, DiagnosticsRelayError> s_startCallback = diagnostics_relay_client_start_service;

        private static readonly ClientNewCallback<DiagnosticsRelayClientHandle, DiagnosticsRelayError> s_clientNewCallback = diagnostics_relay_client_new;

        public DiagnosticsRelaySessionBase(IDevice device) : base(device, s_startCallback)
        {

        }

        public DiagnosticsRelaySessionBase(IDevice device, string serviceID, bool withEscrowBag) : base(device, serviceID, withEscrowBag, s_clientNewCallback)
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

            PlistNode result;
            using (var pArray = new PlistArray())
            {
                pArray.Add(new PlistString(key));
                using var dic = QueryMobilegestalt(pArray);
                result = dic[key].Clone();
            }
            return result;
        }

        public PlistDictionary QueryMobilegestalt(params string[] keys)
        {
            using var pArray = new PlistArray(keys.Select((k) => new PlistString(k)));
            return QueryMobilegestalt(pArray);
        }

        public PlistDictionary QueryMobilegestalt(PlistArray keys)
        {
            DiagnosticsRelayError hresult;
            PlistHandle result;
            hresult = diagnostics_relay_query_mobilegestalt(Handle, keys.Handle, out result);
            if (hresult.IsError())
                throw hresult.GetException();
            using var dic = (PlistDictionary)PlistNode.From(result)!;
            return (PlistDictionary)dic["MobileGestalt"].Clone();
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
