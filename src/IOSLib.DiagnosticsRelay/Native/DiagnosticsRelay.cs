﻿using IOSLib.Native;
using IOSLib.PropertyList.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib.DiagnosticsRelay.Native
{
    class DiagnosticsRelay
    {
        public const string LibraryName = "imobiledevice";

        static DiagnosticsRelay()
        {
            LibraryResolver.EnsureRegistered();
        }

        /// <summary>
        /// Connects to the diagnostics_relay service on the specified device.
        /// </summary>
        /// <param name="device">
        /// The device to connect to.
        /// </param>
        /// <param name="service">
        /// The service descriptor returned by lockdownd_start_service.
        /// </param>
        /// <param name="client">
        /// Reference that will point to a newly allocated
        /// diagnostics_relay_client_t upon successful return.
        /// </param>
        /// <returns>
        /// DIAGNOSTICS_RELAY_E_SUCCESS on success,
        /// DIAGNOSTICS_RELAY_E_INVALID_ARG when one of the parameters is invalid,
        /// or DIAGNOSTICS_RELAY_E_MUX_ERROR when the connection failed.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(DiagnosticsRelay.LibraryName, EntryPoint = "diagnostics_relay_client_new", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern DiagnosticsRelayError diagnostics_relay_client_new(IDeviceHandle device, LockdownServiceDescriptorHandle service, out DiagnosticsRelayClientHandle client);

        /// <summary>
        /// Starts a new diagnostics_relay service on the specified device and connects to it.
        /// </summary>
        /// <param name="device">
        /// The device to connect to.
        /// </param>
        /// <param name="client">
        /// Pointer that will point to a newly allocated
        /// diagnostics_relay_client_t upon successful return. Must be freed using
        /// diagnostics_relay_client_free() after use.
        /// </param>
        /// <param name="label">
        /// The label to use for communication. Usually the program name.
        /// Pass NULL to disable sending the label in requests to lockdownd.
        /// </param>
        /// <returns>
        /// DIAGNOSTICS_RELAY_E_SUCCESS on success, or an DIAGNOSTICS_RELAY_E_* error
        /// code otherwise.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(DiagnosticsRelay.LibraryName, EntryPoint = "diagnostics_relay_client_start_service", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern DiagnosticsRelayError diagnostics_relay_client_start_service(IDeviceHandle device, out DiagnosticsRelayClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string label);

        /// <summary>
        /// Disconnects a diagnostics_relay client from the device and frees up the
        /// diagnostics_relay client data.
        /// </summary>
        /// <param name="client">
        /// The diagnostics_relay client to disconnect and free.
        /// </param>
        /// <returns>
        /// DIAGNOSTICS_RELAY_E_SUCCESS on success,
        /// DIAGNOSTICS_RELAY_E_INVALID_ARG when one of client or client->parent
        /// is invalid, or DIAGNOSTICS_RELAY_E_UNKNOWN_ERROR when the was an
        /// error freeing the parent property_list_service client.
        /// </returns>
        [CompilerServices.GenerateHandle("DiagnosticsRelayClient")]
        [System.Runtime.InteropServices.DllImportAttribute(DiagnosticsRelay.LibraryName, EntryPoint = "diagnostics_relay_client_free", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern DiagnosticsRelayError diagnostics_relay_client_free(System.IntPtr client);

        /// <summary>
        /// Sends the Goodbye request signaling the end of communication.
        /// </summary>
        /// <param name="client">
        /// The diagnostics_relay client
        /// </param>
        /// <returns>
        /// DIAGNOSTICS_RELAY_E_SUCCESS on success,
        /// DIAGNOSTICS_RELAY_E_INVALID_ARG when client is NULL,
        /// DIAGNOSTICS_RELAY_E_PLIST_ERROR if the device did not acknowledge the
        /// request
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(DiagnosticsRelay.LibraryName, EntryPoint = "diagnostics_relay_goodbye", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern DiagnosticsRelayError diagnostics_relay_goodbye(DiagnosticsRelayClientHandle client);

        /// <summary>
        /// Puts the device into deep sleep mode and disconnects from host.
        /// </summary>
        /// <param name="client">
        /// The diagnostics_relay client
        /// </param>
        /// <returns>
        /// DIAGNOSTICS_RELAY_E_SUCCESS on success,
        /// DIAGNOSTICS_RELAY_E_INVALID_ARG when client is NULL,
        /// DIAGNOSTICS_RELAY_E_PLIST_ERROR if the device did not acknowledge the
        /// request
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(DiagnosticsRelay.LibraryName, EntryPoint = "diagnostics_relay_sleep", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern DiagnosticsRelayError diagnostics_relay_sleep(DiagnosticsRelayClientHandle client);

        /// <summary>
        /// Restart the device and optionally show a user notification.
        /// </summary>
        /// <param name="client">
        /// The diagnostics_relay client
        /// </param>
        /// <param name="flags">
        /// A binary flag combination of
        /// DIAGNOSTICS_RELAY_ACTION_FLAG_WAIT_FOR_DISCONNECT to wait until
        /// diagnostics_relay_client_free() disconnects before execution and
        /// DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_FAIL to show a "FAIL" dialog
        /// or DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_PASS to show an "OK" dialog
        /// </param>
        /// <returns>
        /// DIAGNOSTICS_RELAY_E_SUCCESS on success,
        /// DIAGNOSTICS_RELAY_E_INVALID_ARG when client is NULL,
        /// DIAGNOSTICS_RELAY_E_PLIST_ERROR if the device did not acknowledge the
        /// request
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(DiagnosticsRelay.LibraryName, EntryPoint = "diagnostics_relay_restart", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern DiagnosticsRelayError diagnostics_relay_restart(DiagnosticsRelayClientHandle client, DiagnosticsRelayAction flags);

        /// <summary>
        /// Shutdown of the device and optionally show a user notification.
        /// </summary>
        /// <param name="client">
        /// The diagnostics_relay client
        /// </param>
        /// <param name="flags">
        /// A binary flag combination of
        /// DIAGNOSTICS_RELAY_ACTION_FLAG_WAIT_FOR_DISCONNECT to wait until
        /// diagnostics_relay_client_free() disconnects before execution and
        /// DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_FAIL to show a "FAIL" dialog
        /// or DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_PASS to show an "OK" dialog
        /// </param>
        /// <returns>
        /// DIAGNOSTICS_RELAY_E_SUCCESS on success,
        /// DIAGNOSTICS_RELAY_E_INVALID_ARG when client is NULL,
        /// DIAGNOSTICS_RELAY_E_PLIST_ERROR if the device did not acknowledge the
        /// request
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(DiagnosticsRelay.LibraryName, EntryPoint = "diagnostics_relay_shutdown", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern DiagnosticsRelayError diagnostics_relay_shutdown(DiagnosticsRelayClientHandle client, DiagnosticsRelayAction flags);

        /// <summary>
        /// Shutdown of the device and optionally show a user notification.
        /// </summary>
        /// <param name="client">
        /// The diagnostics_relay client
        /// </param>
        /// <param name="flags">
        /// A binary flag combination of
        /// DIAGNOSTICS_RELAY_ACTION_FLAG_WAIT_FOR_DISCONNECT to wait until
        /// diagnostics_relay_client_free() disconnects before execution and
        /// DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_FAIL to show a "FAIL" dialog
        /// or DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_PASS to show an "OK" dialog
        /// </param>
        /// <returns>
        /// DIAGNOSTICS_RELAY_E_SUCCESS on success,
        /// DIAGNOSTICS_RELAY_E_INVALID_ARG when client is NULL,
        /// DIAGNOSTICS_RELAY_E_PLIST_ERROR if the device did not acknowledge the
        /// request
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(DiagnosticsRelay.LibraryName, EntryPoint = "diagnostics_relay_request_diagnostics", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern DiagnosticsRelayError diagnostics_relay_request_diagnostics(DiagnosticsRelayClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string type, out PlistHandle diagnostics);

        [System.Runtime.InteropServices.DllImportAttribute(DiagnosticsRelay.LibraryName, EntryPoint = "diagnostics_relay_query_mobilegestalt", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern DiagnosticsRelayError diagnostics_relay_query_mobilegestalt(DiagnosticsRelayClientHandle client, PlistHandle keys, out PlistHandle result);

        [System.Runtime.InteropServices.DllImportAttribute(DiagnosticsRelay.LibraryName, EntryPoint = "diagnostics_relay_query_ioregistry_entry", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern DiagnosticsRelayError diagnostics_relay_query_ioregistry_entry(DiagnosticsRelayClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string entryName, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string entryClass, out PlistHandle result);

        [System.Runtime.InteropServices.DllImportAttribute(DiagnosticsRelay.LibraryName, EntryPoint = "diagnostics_relay_query_ioregistry_plane", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern DiagnosticsRelayError diagnostics_relay_query_ioregistry_plane(DiagnosticsRelayClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string plane, out PlistHandle result);
    }
}
