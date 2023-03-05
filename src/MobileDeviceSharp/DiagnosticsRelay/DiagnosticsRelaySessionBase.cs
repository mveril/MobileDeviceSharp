using System;
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
    /// <summary>
    /// Represente a session of the <see href="https://docs.libimobiledevice.org/libimobiledevice/latest/diagnostics__relay_8h.html">DiagnosticsRelay</see> service.
    /// </summary>
    public abstract class DiagnosticsRelaySessionBase : ServiceSessionBase<DiagnosticsRelayClientHandle,DiagnosticsRelayError>
    {

        private static readonly StartServiceCallback<DiagnosticsRelayClientHandle, DiagnosticsRelayError> s_startCallback = diagnostics_relay_client_start_service;

        private static readonly ClientNewCallback<DiagnosticsRelayClientHandle, DiagnosticsRelayError> s_clientNewCallback = diagnostics_relay_client_new;

        /// <summary>
        /// Initialize the service using the specified <paramref name="device"/>.
        /// </summary>
        /// <param name="device">The target device</param>
        public DiagnosticsRelaySessionBase(IDevice device) : base(device, s_startCallback)
        {

        }

        /// <summary>
        /// Initialize the service using the specified <paramref name="device"/> and <paramref name="withEscrowBag"/>.
        /// </summary>
        /// <param name="device">The target device.</param>
        /// <param name="serviceID">The service ID.</param>
        /// <param name="withEscrowBag">If <see langword="true"/> use escrowbag.</param>
        public DiagnosticsRelaySessionBase(IDevice device, string serviceID, bool withEscrowBag) : base(device, serviceID, withEscrowBag, s_clientNewCallback)
        {

        }

        /// <summary>
        /// Puts the device into deep sleep mode and disconnects from host.
        /// </summary>
        public void Sleep()
        {
            diagnostics_relay_sleep(Handle);
        }

        /// <summary>
        /// Restart the device and optionally show a user notification.
        /// </summary>
        /// <param name="action">Action to be done.</param>
        public void Reboot(DiagnosticsRelayAction action)
        {
            var hresult = diagnostics_relay_restart(Handle, action);
            if (hresult.IsError())
                throw hresult.GetException();
        }

        /// <summary>
        /// Shutdown of the device and optionally show a user notification.
        /// </summary>
        /// <param name="action">Action to be done.</param>
        public void Shutdown(DiagnosticsRelayAction action)
        {
            var hresult = diagnostics_relay_shutdown(Handle, action);
            if (hresult.IsError())
                throw hresult.GetException();
        }

        /// <summary>
        /// Request diagnostics information for a given type.
        /// </summary>
        /// <param name="type">The type of diagnostics data</param>
        /// <returns>The plist node containing diagnostics data</returns>
        public PlistNode RequestDiagnostics(string type)
        {
            var hresult = diagnostics_relay_request_diagnostics(Handle, type, out var plistHandle);
            if (hresult.IsError())
                throw hresult.GetException();
            return PlistNode.From(plistHandle);
        }

        /// <summary>
        /// Query one property from the MobileGestalt private library.
        /// </summary>
        /// <param name="key">The key corresponding to the requested value from MobileGestalt.</param>
        /// <returns>A <see cref="PlistNode"/> containing the values asociated to the requiested <paramref name="key"/></returns>
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

        /// <summary>
        /// Query some properties from the MobileGestalt private library.
        /// </summary>
        /// <param name="keys"> An array containing the MobileGestalt property keys</param>
        /// <returns>A <see cref="PlistDictionary"/> containing the values asociated to the requiested <paramref name="keys"/></returns>
        public PlistDictionary QueryMobilegestalt(params string[] keys)
        {
            using var pArray = new PlistArray(keys.Select((k) => new PlistString(k)));
            return QueryMobilegestalt(pArray);
        }

        /// <summary>
        /// Query some properties from the MobileGestalt private library.
        /// </summary>
        /// <param name="keys"> An <see cref="PlistArray"/> containing the MobileGestalt property keys</param>
        /// <returns>A <see cref="PlistDictionary"/> containing the values asociated to the requiested <paramref name="keys"/></returns>
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

        /// <summary>
        /// Query the IO registry for a specific entry and return its properties.
        /// </summary>
        /// <param name="entryName">The name of the IO registry entry to query.</param>
        /// <param name="entryClass">The class of the IO registry entry to query.</param>
        /// <returns>A <see cref="PlistNode"/> object containing the properties of the queried IO registry entry.</returns>
        public PlistNode QueryIoregistryEntry(string entryName, string entryClass)
        {
            var hresult = diagnostics_relay_query_ioregistry_entry(Handle, entryName, entryClass, out var result);
            if (hresult.IsError())
                throw hresult.GetException();
            return PlistNode.From(result);
        }

        /// <summary>
        /// Retrieves the IO registry entries for a given service plane on an iOS device connected to a computer.
        /// </summary>
        /// <param name="plane">The name of the service plane for which you want to retrieve information from the IO registry.</param>
        /// <returns>A <see cref="PlistNode"/> object containing the retrieved information in Property List format.</returns>
        public PlistNode QueryIoregistryPlane(string plane)
        {
            var hresult = diagnostics_relay_query_ioregistry_plane(Handle, plane, out var result);
            if (hresult.IsError())
                throw hresult.GetException();
            return PlistNode.From(result);
        }
    }
}
