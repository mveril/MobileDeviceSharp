#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using MobileDeviceSharp.CompilerServices;
using MobileDeviceSharp.Usbmuxd.Native;

namespace MobileDeviceSharp.Native
{
    internal static partial class IDevice
    {
        /// <summary>
        /// Sets the callback to invoke when writing out debug messages. If this callback is set, messages
        /// will be written to this callback instead of the standard output.
        /// </summary>
        /// <param name="callback">
        /// The callback which will receive the debug messages. Set to NULL to redirect to stdout.
        /// </param>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_set_debug_callback")]
        public static partial void idevice_set_debug_callback(IDeviceDebugCallBack callback);

        /// <summary>
        /// Set the level of debugging.
        /// </summary>
        /// <param name="level">
        /// Set to 0 for no debug output, 1 to enable basic debug output and 2 to enable full debug output.
        /// When set to 2, the values of buffers being sent across the wire are printed out as well, this results in very
        /// verbose output.
        /// </param>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_set_debug_level")]
        public static partial void idevice_set_debug_level(int level);

        /// <summary>
        /// Register a callback function that will be called when device add/remove
        /// events occur.
        /// </summary>
        /// <param name="callback">
        /// Callback function to call.
        /// </param>
        /// <param name="userData">
        /// Application-specific data passed as parameter
        /// to the registered callback function.
        /// </param>
        /// <returns>
        /// IDEVICE_E_SUCCESS on success or an error value when an error occurred.
        /// </returns>
        //[LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_event_subscribe")]
        //public static partial IDeviceError idevice_event_subscribe(IDeviceEventCallBack callback, System.IntPtr userData);
        
        /// <summary>
        /// Release the event callback function that has been registered with
        /// idevice_event_subscribe().
        /// </summary>
        /// <returns>
        /// IDEVICE_E_SUCCESS on success or an error value when an error occurred.
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_event_unsubscribe")]
        public static partial IDeviceError idevice_event_unsubscribe();

        /// <summary>
        /// Get a list of UDIDs of currently available devices (USBMUX devices only).
        /// </summary>
        /// <param name="devices">
        /// List of UDIDs of devices that are currently available.
        /// This list is terminated by a NULL pointer.
        /// </param>
        /// <param name="count">
        /// Number of devices found.
        /// </param>
        /// <returns>
        /// IDEVICE_E_SUCCESS on success or an error value when an error occurred.
        /// </returns>
        /// <remarks>
        /// This function only returns the UDIDs of USBMUX devices. To also include
        /// network devices in the list, use idevice_get_device_list_extended().
        /// </remarks>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_get_device_list", StringMarshalling = StringMarshalling.Utf8)]
        public static partial IDeviceError idevice_get_device_list([MarshalUsing(typeof(IDeviceListMarshaller))] out string[] devices, out int count);
        
        /// <summary>
        /// Free a list of device UDIDs.
        /// </summary>
        /// <param name="devices">
        /// List of UDIDs to free.
        /// </param>
        /// <returns>
        /// Always returnes IDEVICE_E_SUCCESS.
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_device_list_free")]
        public static partial IDeviceError idevice_device_list_free(System.IntPtr devices);
        
        /// <summary>
        /// Get a list of currently available devices
        /// </summary>
        /// <param name="devices">
        /// List of idevice_info_t records with device information.
        /// This list is terminated by a NULL pointer.
        /// </param>
        /// <param name="count">
        /// Number of devices included in the list.
        /// </param>
        /// <returns>
        /// IDEVICE_E_SUCCESS on success or an error value when an error occurred.
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_get_device_list_extended", StringMarshalling = StringMarshalling.Utf8)]
        public static partial IDeviceError idevice_get_device_list_extended([MarshalUsing(typeof(IDeviceListExtendedMarshaller))] out string[] devices, out int count);

        /// <summary>
        /// Free an extended device list retrieved through idevice_get_device_list_extended().
        /// </summary>
        /// <param name="devices">
        /// Device list to free.
        /// </param>
        /// <returns>
        /// IDEVICE_E_SUCCESS on success or an error value when an error occurred.
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_device_list_extended_free")]
        public static partial IDeviceError idevice_device_list_extended_free(System.IntPtr devices);
        
        /// <summary>
        /// Creates an idevice_t structure for the device specified by UDID,
        /// if the device is available (USBMUX devices only).
        /// </summary>
        /// <param name="device">
        /// Upon calling this function, a pointer to a location of type
        /// idevice_t. On successful return, this location will be populated.
        /// </param>
        /// <param name="udid">
        /// The UDID to match.
        /// </param>
        /// <returns>
        /// IDEVICE_E_SUCCESS if ok, otherwise an error code.
        /// </returns>
        /// <remarks>
        /// The resulting idevice_t structure has to be freed with
        /// idevice_free() if it is no longer used.
        /// If you need to connect to a device available via network, use
        /// idevice_new_with_options() and include IDEVICE_LOOKUP_NETWORK in options.
        /// </remarks>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_new")]
        public static partial IDeviceError idevice_new(out IDeviceHandle device, [MarshalAsAttribute(UnmanagedType.LPStr)] string udid);
        
        /// <summary>
        /// Creates an idevice_t structure for the device specified by UDID,
        /// if the device is available, with the given lookup options.
        /// </summary>
        /// <param name="device">
        /// Upon calling this function, a pointer to a location of type
        /// idevice_t. On successful return, this location will be populated.
        /// </param>
        /// <param name="udid">
        /// The UDID to match.
        /// </param>
        /// <param name="options">
        /// Specifies what connection types should be considered
        /// when looking up devices. Accepts bitwise or'ed values of idevice_options.
        /// If 0 (no option) is specified it will default to IDEVICE_LOOKUP_USBMUX.
        /// To lookup both USB and network-connected devices, pass
        /// IDEVICE_LOOKUP_USBMUX | IDEVICE_LOOKUP_NETWORK. If a device is available
        /// both via USBMUX *and* network, it will select the USB connection.
        /// This behavior can be changed by adding IDEVICE_LOOKUP_PREFER_NETWORK
        /// to the options in which case it will select the network connection.
        /// </param>
        /// <returns>
        /// IDEVICE_E_SUCCESS if ok, otherwise an error code.
        /// </returns>
        /// <remarks>
        /// The resulting idevice_t structure has to be freed with
        /// idevice_free() if it is no longer used.
        /// </remarks>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_new_with_options")]
        public static partial IDeviceError idevice_new_with_options(out IDeviceHandle device, [MarshalAsAttribute(UnmanagedType.LPStr)] string udid, IDeviceLookupOptions options);
        
        /// <summary>
        /// Cleans up an idevice structure, then frees the structure itself.
        /// </summary>
        /// <param name="device">
        /// idevice_t to free.
        /// </param>
        [UsedForRelease<IDeviceHandle>]
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_free")]
        public static partial IDeviceError idevice_free(System.IntPtr device);
        
        /// <summary>
        /// Set up a connection to the given device.
        /// </summary>
        /// <param name="device">
        /// The device to connect to.
        /// </param>
        /// <param name="port">
        /// The destination port to connect to.
        /// </param>
        /// <param name="connection">
        /// Pointer to an idevice_connection_t that will be filled
        /// with the necessary data of the connection.
        /// </param>
        /// <returns>
        /// IDEVICE_E_SUCCESS if ok, otherwise an error code.
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_connect")]
        public static partial IDeviceError idevice_connect(IDeviceHandle device, ushort port, out IDeviceConnectionHandle connection);

        /// <summary>
        /// Disconnect from the device and clean up the connection structure.
        /// </summary>
        /// <param name="connection">
        /// The connection to close.
        /// </param>
        /// <returns>
        /// IDEVICE_E_SUCCESS if ok, otherwise an error code.
        /// </returns>
        [UsedForRelease<IDeviceConnectionHandle>]
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_disconnect")]
        public static partial IDeviceError idevice_disconnect(System.IntPtr connection);

        /// <summary>
        /// Send data to a device via the given connection.
        /// </summary>
        /// <param name="connection">
        /// The connection to send data over.
        /// </param>
        /// <param name="data">
        /// Buffer with data to send.
        /// </param>
        /// <param name="len">
        /// Size of the buffer to send.
        /// </param>
        /// <param name="sentBytes">
        /// Pointer to an uint32_t that will be filled
        /// with the number of bytes actually sent.
        /// </param>
        /// <returns>
        /// IDEVICE_E_SUCCESS if ok, otherwise an error code.
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_connection_send")]
        public static partial IDeviceError idevice_connection_send(IDeviceConnectionHandle connection, Span<byte> data, uint len, ref uint sentBytes);

        /// <summary>
        /// Receive data from a device via the given connection.
        /// This function will return after the given timeout even if no data has been
        /// received.
        /// </summary>
        /// <param name="connection">
        /// The connection to receive data from.
        /// </param>
        /// <param name="data">
        /// Buffer that will be filled with the received data.
        /// This buffer has to be large enough to hold len bytes.
        /// </param>
        /// <param name="len">
        /// Buffer size or number of bytes to receive.
        /// </param>
        /// <param name="recvBytes">
        /// Number of bytes actually received.
        /// </param>
        /// <param name="timeout">
        /// Timeout in milliseconds after which this function should
        /// return even if no data has been received.
        /// </param>
        /// <returns>
        /// IDEVICE_E_SUCCESS if ok, otherwise an error code.
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_connection_receive_timeout")]
        public static partial IDeviceError idevice_connection_receive_timeout(IDeviceConnectionHandle connection, Span<Byte> data, uint len, ref uint recvBytes, uint timeout);

        /// <summary>
        /// Receive data from a device via the given connection.
        /// This function is like idevice_connection_receive_timeout, but with a
        /// predefined reasonable timeout.
        /// </summary>
        /// <param name="connection">
        /// The connection to receive data from.
        /// </param>
        /// <param name="data">
        /// Buffer that will be filled with the received data.
        /// This buffer has to be large enough to hold len bytes.
        /// </param>
        /// <param name="len">
        /// Buffer size or number of bytes to receive.
        /// </param>
        /// <param name="recvBytes">
        /// Number of bytes actually received.
        /// </param>
        /// <returns>
        /// IDEVICE_E_SUCCESS if ok, otherwise an error code.
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_connection_receive")]
        public static partial IDeviceError idevice_connection_receive(IDeviceConnectionHandle connection, Span<Byte> data, uint len, ref uint recvBytes);

        /// <summary>
        /// Enables SSL for the given connection.
        /// </summary>
        /// <param name="connection">
        /// The connection to enable SSL for.
        /// </param>
        /// <returns>
        /// IDEVICE_E_SUCCESS on success, IDEVICE_E_INVALID_ARG when connection
        /// is NULL or connection->ssl_data is non-NULL, or IDEVICE_E_SSL_ERROR when
        /// SSL initialization, setup, or handshake fails.
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_connection_enable_ssl")]
        public static partial IDeviceError idevice_connection_enable_ssl(IDeviceConnectionHandle connection);

        /// <summary>
        /// Disable SSL for the given connection.
        /// </summary>
        /// <param name="connection">
        /// The connection to disable SSL for.
        /// </param>
        /// <returns>
        /// IDEVICE_E_SUCCESS on success, IDEVICE_E_INVALID_ARG when connection
        /// is NULL. This function also returns IDEVICE_E_SUCCESS when SSL is not
        /// enabled and does no further error checking on cleanup.
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_connection_disable_ssl")]
        public static partial IDeviceError idevice_connection_disable_ssl(IDeviceConnectionHandle connection);

        /// <summary>
        /// Disable bypass SSL for the given connection without sending out terminate messages.
        /// </summary>
        /// <param name="connection">
        /// The connection to disable SSL for.
        /// </param>
        /// <param name="sslBypass">
        /// if true ssl connection will not be terminated but just cleaned up, allowing
        /// plain text data going on underlying connection
        /// </param>
        /// <returns>
        /// IDEVICE_E_SUCCESS on success, IDEVICE_E_INVALID_ARG when connection
        /// is NULL. This function also returns IDEVICE_E_SUCCESS when SSL is not
        /// enabled and does no further error checking on cleanup.
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_connection_disable_bypass_ssl")]
        public static partial IDeviceError idevice_connection_disable_bypass_ssl(IDeviceConnectionHandle connection, [MarshalAs(UnmanagedType.I1)] bool sslBypass);

        /// <summary>
        /// Get the underlying file descriptor for a connection
        /// </summary>
        /// <param name="connection">
        /// The connection to get fd of
        /// </param>
        /// <param name="fd">
        /// Pointer to an int where the fd is stored
        /// </param>
        /// <returns>
        /// IDEVICE_E_SUCCESS if ok, otherwise an error code.
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_connection_get_fd")]
        public static partial IDeviceError idevice_connection_get_fd(IDeviceConnectionHandle connection, ref int fd);

        /// <summary>
        /// Gets the handle or (usbmux device id) of the device.
        /// </summary>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_get_handle")]
        public static partial IDeviceError idevice_get_handle(IDeviceHandle device, ref uint handle);

        /// <summary>
        /// Gets the unique id for the device.
        /// </summary>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_get_udid")]
        public static partial IDeviceError idevice_get_udid(IDeviceHandle device, [MarshalAs(UnmanagedType.LPUTF8Str)] out string udid);

        /// <summary>
        /// Sets the socket type (Unix socket or TCP socket) libimobiledevice should use when connecting
        /// to usbmuxd.
        /// </summary>
        /// <param name="value">
        /// IDEVICE_SOCKET_TYPE_UNIX or IDEVICE_SOCKET_TYPE_TCP
        /// </param>
        /// <returns>
        /// 0 on success or negative on error
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_set_socket_type")]
        public static partial IDeviceError idevice_set_socket_type(int value);

        /// <summary>
        /// Gets the socket type (Unix socket or TCP socket) libimobiledevice should use when connecting
        /// to usbmuxd.
        /// </summary>
        /// <param name="value">
        /// A pointer to an integer which will reveive the current socket type
        /// </param>
        /// <returns>
        /// 0 on success or negative on error
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_get_socket_type")]
        public static partial IDeviceError idevice_get_socket_type(ref int value);

        /// <summary>
        /// Sets the TCP endpoint to which libimobiledevice will connect if the socket type is set to
        /// SOCKET_TYPE_TCP
        /// </summary>
        /// <param name="host">
        /// The hostname or IP address to which to connect
        /// </param>
        /// <param name="port">
        /// The port to which to connect.
        /// </param>
        /// <returns>
        /// 0 on success or negative on error
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_set_tcp_endpoint")]
        public static partial IDeviceError idevice_set_tcp_endpoint([MarshalAsAttribute(UnmanagedType.LPStr)] string host, ushort port);

        /// <summary>
        /// Gets the TCP endpoint to which libimobiledevice will connect if the socket type is set to
        /// SOCKET_TYPE_TCP
        /// </summary>
        /// <param name="host">
        /// A pointer which will be set to the hostname or IP address to which to connect.
        /// The caller must free this string.
        /// </param>
        /// <param name="port">
        /// The port to which to connect
        /// </param>
        /// <returns>
        /// 0 on success or negative on error
        /// </returns>
        [LibraryImportAttribute(IDevice.LibraryName, EntryPoint = "idevice_get_tcp_endpoint")]
        public static partial IDeviceError idevice_get_tcp_endpoint([MarshalAs(UnmanagedType.LPUTF8Str)] out string host, ref ushort port);
    }
}
#endif
