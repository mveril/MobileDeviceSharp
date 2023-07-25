﻿#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.CompilerServices;
using System.Runtime.InteropServices;
using MobileDeviceSharp.PropertyList;
using MobileDeviceSharp.PropertyList.Native;

namespace MobileDeviceSharp.Native
{
    internal static partial class Lockdown
    {
        /// <summary>
        /// Creates a new lockdownd client for the device.
        /// </summary>
        /// <param name="device">
        /// The device to create a lockdownd client for
        /// </param>
        /// <param name="client">
        /// The pointer to the location of the new lockdownd_client
        /// </param>
        /// <param name="label">
        /// The label to use for communication. Usually the program name.
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client is NULL
        /// </returns>
        /// <remarks>
        /// This function does not pair with the device or start a session. This
        /// has to be done manually by the caller after the client is created.
        /// The device disconnects automatically if the lockdown connection idles
        /// for more than 10 seconds. Make sure to call lockdownd_client_free() as soon
        /// as the connection is no longer needed.
        /// </remarks>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_client_new")]
        public static partial LockdownError lockdownd_client_new(IDeviceHandle device, out LockdownClientHandle client, [MarshalAs(UnmanagedType.LPStr)] string? label);

        /// <summary>
        /// Creates a new lockdownd client for the device and starts initial handshake.
        /// The handshake consists out of query_type, validate_pair, pair and
        /// start_session calls. It uses the internal pairing record management.
        /// </summary>
        /// <param name="device">
        /// The device to create a lockdownd client for
        /// </param>
        /// <param name="client">
        /// The pointer to the location of the new lockdownd_client
        /// </param>
        /// <param name="label">
        /// The label to use for communication. Usually the program name.
        /// Pass NULL to disable sending the label in requests to lockdownd.
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client is NULL,
        /// LOCKDOWN_E_INVALID_CONF if configuration data is wrong
        /// </returns>
        /// <remarks>
        /// The device disconnects automatically if the lockdown connection idles
        /// for more than 10 seconds. Make sure to call lockdownd_client_free() as soon
        /// as the connection is no longer needed.
        /// </remarks>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_client_new_with_handshake")]
        public static partial LockdownError lockdownd_client_new_with_handshake(IDeviceHandle device, out LockdownClientHandle client, [MarshalAs(UnmanagedType.LPStr)] string? label);

        /// <summary>
        /// Closes the lockdownd client session if one is running and frees up the
        /// lockdownd_client struct.
        /// </summary>
        /// <param name="client">
        /// The lockdown client
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client is NULL
        /// </returns>
        [UsedForRelease<LockdownClientHandle>]
        [LibraryImport(Lockdown.LibraryName, EntryPoint = "lockdownd_client_free")]
        public static partial LockdownError lockdownd_client_free(System.IntPtr client);

        /// <summary>
        /// Query the type of the service daemon. Depending on whether the device is
        /// queried in normal mode or restore mode, different types will be returned.
        /// </summary>
        /// <param name="client">
        /// The lockdownd client
        /// </param>
        /// <param name="type">
        /// The type returned by the service daemon. Pass NULL to ignore.
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client is NULL
        /// </returns>
        [LibraryImport(Lockdown.LibraryName, EntryPoint = "lockdownd_query_type", StringMarshalling = StringMarshalling.Utf8)]
        public static partial LockdownError lockdownd_query_type(LockdownClientHandle client, out string type);

        /// <summary>
        /// Retrieves a preferences plist using an optional domain and/or key name.
        /// </summary>
        /// <param name="client">
        /// An initialized lockdownd client.
        /// </param>
        /// <param name="domain">
        /// The domain to query on or NULL for global domain
        /// </param>
        /// <param name="key">
        /// The key name to request or NULL to query for all keys
        /// </param>
        /// <param name="value">
        /// A plist node representing the result value node
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client is NULL
        /// </returns>
        [LibraryImport(Lockdown.LibraryName, EntryPoint = "lockdownd_get_value")]
        public static partial LockdownError lockdownd_get_value(LockdownClientHandle client, [MarshalAsAttribute(UnmanagedType.LPStr)] string? domain, [MarshalAsAttribute(UnmanagedType.LPStr)] string? key, out PlistHandle value);

        /// <summary>
        /// Sets a preferences value using a plist and optional by domain and/or key name.
        /// </summary>
        /// <param name="client">
        /// an initialized lockdownd client.
        /// </param>
        /// <param name="domain">
        /// the domain to query on or NULL for global domain
        /// </param>
        /// <param name="key">
        /// the key name to set the value or NULL to set a value dict plist
        /// </param>
        /// <param name="value">
        /// a plist node of any node type representing the value to set
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client or
        /// value is NULL
        /// </returns>
        [LibraryImport(Lockdown.LibraryName, EntryPoint = "lockdownd_set_value")]
        public static partial LockdownError lockdownd_set_value(LockdownClientHandle client, [MarshalAsAttribute(UnmanagedType.LPStr)] string? domain, [MarshalAsAttribute(UnmanagedType.LPStr)] string key, PlistHandle value);

        /// <summary>
        /// Removes a preference node by domain and/or key name.
        /// </summary>
        /// <param name="client">
        /// An initialized lockdownd client.
        /// </param>
        /// <param name="domain">
        /// The domain to query on or NULL for global domain
        /// </param>
        /// <param name="key">
        /// The key name to remove or NULL remove all keys for the current domain
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client is NULL
        /// </returns>
        /// <remarks>
        /// : Use with caution as this could remove vital information on the device
        /// </remarks>
        [LibraryImport(Lockdown.LibraryName, EntryPoint = "lockdownd_remove_value")]
        public static partial LockdownError lockdownd_remove_value(LockdownClientHandle client, [MarshalAsAttribute(UnmanagedType.LPStr)] string domain, [MarshalAsAttribute(UnmanagedType.LPStr)] string key);

        /// <summary>
        /// Requests to start a service and retrieve it's port on success.
        /// </summary>
        /// <param name="client">
        /// The lockdownd client
        /// </param>
        /// <param name="identifier">
        /// The identifier of the service to start
        /// </param>
        /// <param name="service">
        /// The service descriptor on success or NULL on failure
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG if a parameter
        /// is NULL, LOCKDOWN_E_INVALID_SERVICE if the requested service is not known
        /// by the device, LOCKDOWN_E_START_SERVICE_FAILED if the service could not be
        /// started by the device
        /// </returns>
        [LibraryImport(Lockdown.LibraryName, EntryPoint = "lockdownd_start_service")]
        public static partial LockdownError lockdownd_start_service(LockdownClientHandle client, [MarshalAsAttribute(UnmanagedType.LPStr)] string identifier, out LockdownServiceDescriptorHandle service);

        /// <summary>
        /// Requests to start a service and retrieve it's port on success.
        /// Sends the escrow bag from the device's pair record.
        /// </summary>
        /// <param name="client">
        /// The lockdownd client
        /// </param>
        /// <param name="identifier">
        /// The identifier of the service to start
        /// </param>
        /// <param name="service">
        /// The service descriptor on success or NULL on failure
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG if a parameter
        /// is NULL, LOCKDOWN_E_INVALID_SERVICE if the requested service is not known
        /// by the device, LOCKDOWN_E_START_SERVICE_FAILED if the service could not because
        /// started by the device, LOCKDOWN_E_INVALID_CONF if the host id or escrow bag are
        /// missing from the device record.
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_start_service_with_escrow_bag")]
        public static partial LockdownError lockdownd_start_service_with_escrow_bag(LockdownClientHandle client, [MarshalAsAttribute(UnmanagedType.LPStr)] string identifier, out LockdownServiceDescriptorHandle service);

        /// <summary>
        /// Opens a session with lockdownd and switches to SSL mode if device wants it.
        /// </summary>
        /// <param name="client">
        /// The lockdownd client
        /// </param>
        /// <param name="hostId">
        /// The HostID of the computer
        /// </param>
        /// <param name="sessionId">
        /// The new session_id of the created session
        /// </param>
        /// <param name="sslEnabled">
        /// Whether SSL communication is used in the session
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when a client
        /// or host_id is NULL, LOCKDOWN_E_PLIST_ERROR if the response plist had errors,
        /// LOCKDOWN_E_INVALID_HOST_ID if the device does not know the supplied HostID,
        /// LOCKDOWN_E_SSL_ERROR if enabling SSL communication failed
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_start_session")]
        public static partial LockdownError lockdownd_start_session(LockdownClientHandle client, [MarshalAsAttribute(UnmanagedType.LPStr)] string hostId, out System.IntPtr sessionId, out int sslEnabled);

        /// <summary>
        /// Closes the lockdownd session by sending the StopSession request.
        /// </summary>
        /// <param name="client">
        /// The lockdown client
        /// </param>
        /// <param name="sessionId">
        /// The id of a running session
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client is NULL
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_stop_session")]
        public static partial LockdownError lockdownd_stop_session(LockdownClientHandle client, [MarshalAsAttribute(UnmanagedType.LPStr)] string sessionId);

        /// <summary>
        /// Sends a plist to lockdownd.
        /// </summary>
        /// <param name="client">
        /// The lockdownd client
        /// </param>
        /// <param name="plist">
        /// The plist to send
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client or
        /// plist is NULL
        /// </returns>
        /// <remarks>
        /// This function is low-level and should only be used if you need to send
        /// a new type of message.
        /// </remarks>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_send")]
        public static partial LockdownError lockdownd_send(LockdownClientHandle client, PlistHandle plist);

        /// <summary>
        /// Receives a plist from lockdownd.
        /// </summary>
        /// <param name="client">
        /// The lockdownd client
        /// </param>
        /// <param name="plist">
        /// The plist to store the received data
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client or
        /// plist is NULL
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_receive")]
        public static partial LockdownError lockdownd_receive(LockdownClientHandle client, out PlistHandle plist);

        /// <summary>
        /// Pairs the device using the supplied pair record.
        /// </summary>
        /// <param name="client">
        /// The lockdown client
        /// </param>
        /// <param name="pairRecord">
        /// The pair record to use for pairing. If NULL is passed, then
        /// the pair records from the current machine are used. New records will be
        /// generated automatically when pairing is done for the first time.
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client is NULL,
        /// LOCKDOWN_E_PLIST_ERROR if the pair_record certificates are wrong,
        /// LOCKDOWN_E_PAIRING_FAILED if the pairing failed,
        /// LOCKDOWN_E_PASSWORD_PROTECTED if the device is password protected,
        /// LOCKDOWN_E_INVALID_HOST_ID if the device does not know the caller's host id
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_pair")]
        public static partial LockdownError lockdownd_pair(LockdownClientHandle client, LockdownPairRecordHandle pairRecord);

        /// <summary>
        /// Pairs the device using the supplied pair record and passing the given options.
        /// </summary>
        /// <param name="client">
        /// The lockdown client
        /// </param>
        /// <param name="pairRecord">
        /// The pair record to use for pairing. If NULL is passed, then
        /// the pair records from the current machine are used. New records will be
        /// generated automatically when pairing is done for the first time.
        /// </param>
        /// <param name="options">
        /// The pairing options to pass. Can be NULL for no options.
        /// </param>
        /// <param name="response">
        /// If non-NULL a pointer to lockdownd's response dictionary is returned.
        /// The caller is responsible to free the response dictionary with plist_free().
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client is NULL,
        /// LOCKDOWN_E_PLIST_ERROR if the pair_record certificates are wrong,
        /// LOCKDOWN_E_PAIRING_FAILED if the pairing failed,
        /// LOCKDOWN_E_PASSWORD_PROTECTED if the device is password protected,
        /// LOCKDOWN_E_INVALID_HOST_ID if the device does not know the caller's host id
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_pair_with_options")]
        public static partial LockdownError lockdownd_pair_with_options(LockdownClientHandle client, LockdownPairRecordHandle pairRecord, PlistHandle options, out PlistHandle response);

        /// <summary>
        /// Validates if the device is paired with the given HostID. If successful the
        /// specified host will become trusted host of the device indicated by the
        /// lockdownd preference named TrustedHostAttached. Otherwise the host must be
        /// paired using lockdownd_pair() first.
        /// </summary>
        /// <param name="client">
        /// The lockdown client
        /// </param>
        /// <param name="pairRecord">
        /// The pair record to validate pairing with. If NULL is
        /// passed, then the pair record is read from the internal pairing record
        /// management.
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client is NULL,
        /// LOCKDOWN_E_PLIST_ERROR if the pair_record certificates are wrong,
        /// LOCKDOWN_E_PAIRING_FAILED if the pairing failed,
        /// LOCKDOWN_E_PASSWORD_PROTECTED if the device is password protected,
        /// LOCKDOWN_E_INVALID_HOST_ID if the device does not know the caller's host id
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_validate_pair")]
        public static partial LockdownError lockdownd_validate_pair(LockdownClientHandle client, LockdownPairRecordHandle pairRecord);

        /// <summary>
        /// Validates if the device is paired with the given HostID. If successful the
        /// specified host will become trusted host of the device indicated by the
        /// lockdownd preference named TrustedHostAttached. Otherwise the host must be
        /// paired using lockdownd_pair() first.
        /// </summary>
        /// <param name="client">
        /// The lockdown client
        /// </param>
        /// <param name="pairRecord">
        /// The pair record to validate pairing with. If NULL is
        /// passed, then the pair record is read from the internal pairing record
        /// management.
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client is NULL,
        /// LOCKDOWN_E_PLIST_ERROR if the pair_record certificates are wrong,
        /// LOCKDOWN_E_PAIRING_FAILED if the pairing failed,
        /// LOCKDOWN_E_PASSWORD_PROTECTED if the device is password protected,
        /// LOCKDOWN_E_INVALID_HOST_ID if the device does not know the caller's host id
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_validate_pair")]
        public unsafe static partial LockdownError lockdownd_validate_pair_intptr(LockdownClientHandle client, void* pairRecord);

        /// <summary>
        /// Unpairs the device with the given HostID and removes the pairing records
        /// from the device and host if the internal pairing record management is used.
        /// </summary>
        /// <param name="client">
        /// The lockdown client
        /// </param>
        /// <param name="pairRecord">
        /// The pair record to use for unpair. If NULL is passed, then
        /// the pair records from the current machine are used.
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client is NULL,
        /// LOCKDOWN_E_PLIST_ERROR if the pair_record certificates are wrong,
        /// LOCKDOWN_E_PAIRING_FAILED if the pairing failed,
        /// LOCKDOWN_E_PASSWORD_PROTECTED if the device is password protected,
        /// LOCKDOWN_E_INVALID_HOST_ID if the device does not know the caller's host id
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_unpair")]
        public static partial LockdownError lockdownd_unpair(LockdownClientHandle client, LockdownPairRecordHandle pairRecord);

        /// <summary>
        /// Activates the device. Only works within an open session.
        /// The ActivationRecord plist dictionary must be obtained using the
        /// activation protocol requesting from Apple's https webservice.
        /// </summary>
        /// <param name="client">
        /// The lockdown client
        /// </param>
        /// <param name="activationRecord">
        /// The activation record plist dictionary
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client or
        /// activation_record is NULL, LOCKDOWN_E_NO_RUNNING_SESSION if no session is
        /// open, LOCKDOWN_E_PLIST_ERROR if the received plist is broken,
        /// LOCKDOWN_E_ACTIVATION_FAILED if the activation failed,
        /// LOCKDOWN_E_INVALID_ACTIVATION_RECORD if the device reports that the
        /// activation_record is invalid
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_activate")]
        public static partial LockdownError lockdownd_activate(LockdownClientHandle client, PlistHandle activationRecord);

        /// <summary>
        /// Deactivates the device, returning it to the locked “Activate with iTunes”
        /// screen.
        /// </summary>
        /// <param name="client">
        /// The lockdown client
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client is NULL,
        /// LOCKDOWN_E_NO_RUNNING_SESSION if no session is open,
        /// LOCKDOWN_E_PLIST_ERROR if the received plist is broken
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_deactivate")]
        public static partial LockdownError lockdownd_deactivate(LockdownClientHandle client);

        /// <summary>
        /// Tells the device to immediately enter recovery mode.
        /// </summary>
        /// <param name="client">
        /// The lockdown client
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client is NULL
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_enter_recovery")]
        public static partial LockdownError lockdownd_enter_recovery(LockdownClientHandle client);

        /// <summary>
        /// Sends the Goodbye request to lockdownd signaling the end of communication.
        /// </summary>
        /// <param name="client">
        /// The lockdown client
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success, LOCKDOWN_E_INVALID_ARG when client
        /// is NULL, LOCKDOWN_E_PLIST_ERROR if the device did not acknowledge the
        /// request
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_goodbye")]
        public static partial LockdownError lockdownd_goodbye(LockdownClientHandle client);

        /// <summary>
        /// Sets the label to send for requests to lockdownd.
        /// </summary>
        /// <param name="client">
        /// The lockdown client
        /// </param>
        /// <param name="label">
        /// The label to set or NULL to disable sending a label
        /// </param>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_client_set_label")]
        public static partial void lockdownd_client_set_label(LockdownClientHandle client, [MarshalAsAttribute(UnmanagedType.LPStr)] string? label);

        /// <summary>
        /// Returns the unique id of the device from lockdownd.
        /// </summary>
        /// <param name="client">
        /// An initialized lockdownd client.
        /// </param>
        /// <param name="udid">
        /// Holds the unique id of the device. The caller is responsible
        /// for freeing the memory.
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_get_device_udid", StringMarshalling = StringMarshalling.Utf8)]
        public static partial LockdownError lockdownd_get_device_udid(LockdownClientHandle client, out string udid);

        /// <summary>
        /// Retrieves the name of the device from lockdownd set by the user.
        /// </summary>
        /// <param name="client">
        /// An initialized lockdownd client.
        /// </param>
        /// <param name="deviceName">
        /// Holds the name of the device. The caller is
        /// responsible for freeing the memory.
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_get_device_name", StringMarshalling = StringMarshalling.Utf8)]
        public static partial LockdownError lockdownd_get_device_name(LockdownClientHandle client, out string deviceName);

        /// <summary>
        /// Calculates and returns the data classes the device supports from lockdownd.
        /// </summary>
        /// <param name="client">
        /// An initialized lockdownd client.
        /// </param>
        /// <param name="classes">
        /// A pointer to store an array of class names. The caller is responsible
        /// for freeing the memory which can be done using mobilesync_data_classes_free().
        /// </param>
        /// <param name="count">
        /// The number of items in the classes array.
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success,
        /// LOCKDOWN_E_INVALID_ARG when client is NULL,
        /// LOCKDOWN_E_NO_RUNNING_SESSION if no session is open,
        /// LOCKDOWN_E_PLIST_ERROR if the received plist is broken
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_get_sync_data_classes")]
        public static partial LockdownError lockdownd_get_sync_data_classes(LockdownClientHandle client, out System.IntPtr classes, ref int count);

        /// <summary>
        /// Frees memory of an allocated array of data classes as returned by lockdownd_get_sync_data_classes()
        /// </summary>
        /// <param name="classes">
        /// An array of class names to free.
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success
        /// </returns>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_data_classes_free")]
        public static partial LockdownError lockdownd_data_classes_free(System.IntPtr classes);

        /// <summary>
        /// Frees memory of a service descriptor as returned by lockdownd_start_service()
        /// </summary>
        /// <param name="service">
        /// A service descriptor instance to free.
        /// </param>
        /// <returns>
        /// LOCKDOWN_E_SUCCESS on success
        /// </returns>
        [UsedForRelease<LockdownServiceDescriptorHandle>]
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_service_descriptor_free")]
        public static partial LockdownError lockdownd_service_descriptor_free(System.IntPtr service);

        /// <summary>
        /// Gets a readable error string for a given lockdown error code.
        /// </summary>
        /// <param name="err">
        /// A lockdownd error code
        /// </param>
        [LibraryImportAttribute(Lockdown.LibraryName, EntryPoint = "lockdownd_strerror", StringMarshalling = StringMarshalling.Utf8)]
        public static partial string lockdownd_strerror(LockdownError err);
    }
}
#endif
