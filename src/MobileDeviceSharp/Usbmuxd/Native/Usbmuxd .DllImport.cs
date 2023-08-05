#if !NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MobileDeviceSharp.CompilerServices;
using MobileDeviceSharp.Native;

namespace MobileDeviceSharp.Usbmuxd.Native
{
    internal static partial class Usbmuxd
    {

        /// <summary>
        /// Subscribe a callback function to be called upon device add/remove events.
        /// This method can be called multiple times to register multiple callbacks
        /// since every subscription will have its own context (returned in the
        /// first parameter).
        /// </summary>
        /// <param name="context">
        /// A pointer to a usbmuxd_subscription_context_t that will be
        /// set upon creation of the subscription. The returned context must be
        /// passed to usbmuxd_events_unsubscribe() to unsubscribe the callback.
        /// </param>
        /// <param name="callback">
        /// A callback function that is executed when an event occurs.
        /// </param>
        /// <param name="userData">
        /// Custom data passed on to the callback function. The data
        /// needs to be kept available until the callback function is unsubscribed.
        /// </param>
        /// <returns>
        /// 0 on success or a negative errno value.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_events_subscribe", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_events_subscribe(out UsbmuxdSubscriptionContextHandle context, 
        
        callback, System.IntPtr userData);

        /// <summary>
        /// Unsubscribe callback function
        /// </summary>
        /// <param name="context">
        /// A valid context as returned from usbmuxd_events_subscribe().
        /// </param>
        /// <returns>
        /// 0 on success or a negative errno value.
        /// </returns>
        [UsedForRelease<UsbmuxdSubscriptionContextHandle>]
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_events_unsubscribe", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_events_unsubscribe(UsbmuxdSubscriptionContextHandle context);

        /// <summary>
        /// Subscribe a callback (deprecated)
        /// </summary>
        /// <param name="callback">
        /// A callback function that is executed when an event occurs.
        /// </param>
        /// <param name="userData">
        /// Custom data passed on to the callback function. The data
        /// needs to be kept available until the callback function is unsubscribed.
        /// </param>
        /// <returns>
        /// 0 on success or negative on error.
        /// </returns>
        /// <remarks>
        /// Deprecated. Use usbmuxd_events_subscribe and usbmuxd_events_unsubscribe instead.
        /// </remarks>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_subscribe", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_subscribe(UsbmuxdEventCallBack callback, System.IntPtr userData);

        /// <summary>
        /// Unsubscribe callback (deprecated)
        /// </summary>
        /// <returns>
        /// 0 on success or negative on error.
        /// </returns>
        /// <remarks>
        /// Deprecated. Use usbmuxd_events_subscribe and usbmuxd_events_unsubscribe instead.
        /// </remarks>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_unsubscribe", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_unsubscribe();

        /// <summary>
        /// Contacts usbmuxd and retrieves a list of connected devices.
        /// </summary>
        /// <param name="deviceList">
        /// A pointer to an array of usbmuxd_device_info_t
        /// that will hold records of the connected devices. The last record
        /// is a null-terminated record with all fields set to 0/NULL.
        /// </param>
        /// <returns>
        /// number of attached devices, zero on no devices, or negative
        /// if an error occured.
        /// </returns>
        /// <remarks>
        /// The user has to free the list returned.
        /// </remarks>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_get_deviceList", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_get_deviceList(out UsbmuxdDeviceInfo[] deviceList);

        /// <summary>
        /// Frees the device list returned by an usbmuxd_get_deviceList call
        /// </summary>
        /// <param name="deviceList">
        /// A pointer to an array of usbmuxd_device_info_t to free.
        /// </param>
        /// <returns>
        /// 0 on success, -1 on error.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_deviceList_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_deviceList_free(System.IntPtr deviceList);

        /// <summary>
        /// Looks up the device specified by UDID and returns device information.
        /// </summary>
        /// <param name="udid">
        /// A device UDID of the device to look for. If udid is NULL,
        /// This function will return the first device found.
        /// </param>
        /// <param name="device">
        /// Pointer to a previously allocated (or static)
        /// usbmuxd_device_info_t that will be filled with the device info.
        /// </param>
        /// <returns>
        /// 0 if no matching device is connected, 1 if the device was found,
        /// or a negative value on error.
        /// </returns>
        /// <remarks>
        /// This function only considers devices connected through USB. To
        /// query devices available via network, use usbmuxd_get_device().
        /// </remarks>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_get_device_by_udid", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_get_device_by_udid([MarshalAsAttribute(UnmanagedType.LPStr)] string udid, out UsbmuxdDeviceInfo device);

        /// <summary>
        /// Looks up the device specified by UDID with given options and returns
        /// device information.
        /// </summary>
        /// <param name="udid">
        /// A device UDID of the device to look for. If udid is NULL,
        /// this function will return the first device found.
        /// </param>
        /// <param name="device">
        /// Pointer to a previously allocated (or static)
        /// usbmuxd_device_info_t that will be filled with the device info.
        /// </param>
        /// <param name="options">
        /// Specifying what device connection types should be
        /// considered during lookup. Accepts bitwise or'ed values of
        /// usbmux_lookup_options.
        /// If 0 (no option) is specified it will default to DEVICE_LOOKUP_USBMUX.
        /// To lookup both USB and network-connected devices, pass
        /// DEVICE_LOOKUP_USBMUX | DEVICE_LOOKUP_NETWORK. If a device is available
        /// both via USBMUX *and* network, it will select the USB connection.
        /// This behavior can be changed by adding DEVICE_LOOKUP_PREFER_NETWORK
        /// to the options in which case it will select the network connection.
        /// </param>
        /// <returns>
        /// 0 if no matching device is connected, 1 if the device was found,
        /// or a negative value on error.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_get_device", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_get_device([MarshalAsAttribute(UnmanagedType.LPStr)] string udid, out UsbmuxdDeviceInfo device, IDeviceLookupOptions options);

        /// <summary>
        /// Request proxy connection to the specified device and port.
        /// </summary>
        /// <param name="handle">
        /// returned in the usbmux_device_info_t structure via
        /// usbmuxd_get_device() or usbmuxd_get_deviceList().
        /// </param>
        /// <param name="tcpPort">
        /// TCP port number on device, in range 0-65535.
        /// common values are 62078 for lockdown, and 22 for SSH.
        /// </param>
        /// <returns>
        /// socket file descriptor of the connection, or a negative errno
        /// value on error.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_connect", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_connect(uint handle, ushort tcpPort);

        /// <summary>
        /// Disconnect. For now, this just closes the socket file descriptor.
        /// </summary>
        /// <param name="sfd">
        /// socket file descriptor returned by usbmuxd_connect()
        /// </param>
        /// <returns>
        /// 0 on success, -1 on error.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_disconnect", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_disconnect(int sfd);

        /// <summary>
        /// Send data to the specified socket.
        /// </summary>
        /// <param name="sfd">
        /// socket file descriptor returned by usbmuxd_connect()
        /// </param>
        /// <param name="data">
        /// buffer to send
        /// </param>
        /// <param name="len">
        /// size of buffer to send
        /// </param>
        /// <param name="sentBytes">
        /// how many bytes sent
        /// </param>
        /// <returns>
        /// 0 on success, a negative errno value otherwise.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_send", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_send(int sfd, byte[] data, uint len, ref uint sentBytes);

        /// <summary>
        /// Receive data from the specified socket.
        /// </summary>
        /// <param name="sfd">
        /// socket file descriptor returned by usbmuxd_connect()
        /// </param>
        /// <param name="data">
        /// buffer to put the data to
        /// </param>
        /// <param name="len">
        /// number of bytes to receive
        /// </param>
        /// <param name="recvBytes">
        /// number of bytes received
        /// </param>
        /// <param name="timeout">
        /// how many milliseconds to wait for data
        /// </param>
        /// <returns>
        /// 0 on success, a negative errno value otherwise.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_recv_timeout", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_recv_timeout(int sfd, byte[] data, uint len, ref uint recvBytes, uint timeout);

        /// <summary>
        /// Receive data from the specified socket with a default timeout.
        /// </summary>
        /// <param name="sfd">
        /// socket file descriptor returned by usbmuxd_connect()
        /// </param>
        /// <param name="data">
        /// buffer to put the data to
        /// </param>
        /// <param name="len">
        /// number of bytes to receive
        /// </param>
        /// <param name="recvBytes">
        /// number of bytes received
        /// </param>
        /// <returns>
        /// 0 on success, a negative errno value otherwise.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_recv", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_recv(int sfd, byte[] data, uint len, ref uint recvBytes);

        /// <summary>
        /// Reads the SystemBUID
        /// </summary>
        /// <param name="buid">
        /// pointer to a variable that will be set to point to a newly
        /// allocated string with the System BUID returned by usbmuxd
        /// </param>
        /// <returns>
        /// 0 on success, a negative errno value otherwise.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_read_buid", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_read_buid([MarshalAs(UnmanagedType.CustomMarshaler,MarshalTypeRef =typeof(UTF8Marshaler))] out string buid);

        /// <summary>
        /// Read a pairing record
        /// </summary>
        /// <param name="recordId">
        /// the record identifier of the pairing record to retrieve
        /// </param>
        /// <param name="recordData">
        /// pointer to a variable that will be set to point to a
        /// newly allocated buffer containing the pairing record data
        /// </param>
        /// <param name="recordSize">
        /// pointer to a variable that will be set to the size of
        /// the buffer returned in recordData
        /// </param>
        /// <returns>
        /// 0 on success, a negative error value otherwise.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_read_pair_record", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_read_pair_record([MarshalAsAttribute(UnmanagedType.LPStr)] string recordId, out System.IntPtr recordData, out uint recordSize);

        /// <summary>
        /// Read a pairing record
        /// </summary>
        /// <param name="recordId">
        /// the record identifier of the pairing record to retrieve
        /// </param>
        /// <param name="recordData">
        /// pointer to a variable that will be set to point to a
        /// newly allocated buffer containing the pairing record data
        /// </param>
        /// <param name="recordSize">
        /// pointer to a variable that will be set to the size of
        /// the buffer returned in recordData
        /// </param>
        /// <returns>
        /// 0 on success, a negative error value otherwise.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_read_pair_record", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int usbmuxd_read_pair_record([MarshalAsAttribute(UnmanagedType.LPStr)] string recordId, out byte* recordData, out uint recordSize);

        /// <summary>
        /// Read a pairing record
        /// </summary>
        /// <param name="recordId">
        /// the record identifier of the pairing record to retrieve
        /// </param>
        /// <param name="recordData">
        /// pointer to a variable that will be set to point to a
        /// newly allocated buffer containing the pairing record data
        /// </param>
        /// <param name="recordSize">
        /// pointer to a variable that will be set to the size of
        /// the buffer returned in recordData
        /// </param>
        /// <returns>
        /// 0 on success, a negative error value otherwise.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_read_pair_record", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int usbmuxd_read_pair_record([MarshalAsAttribute(UnmanagedType.LPStr)] string recordId, out byte[] recordData, out uint recordSize);

        /// <summary>
        /// Save a pairing record
        /// </summary>
        /// <param name="recordId">
        /// the record identifier of the pairing record to save
        /// </param>
        /// <param name="recordData">
        /// buffer containing the pairing record data
        /// </param>
        /// <param name="recordSize">
        /// size of the buffer passed in recordData
        /// </param>
        /// <returns>
        /// 0 on success, a negative error value otherwise.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_save_pair_record", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_save_pair_record([MarshalAsAttribute(UnmanagedType.LPStr)] string recordId, [MarshalAsAttribute(UnmanagedType.LPStr)] System.IntPtr recordData, uint recordSize);

        /// <summary>
        /// Save a pairing record
        /// </summary>
        /// <param name="recordId">
        /// the record identifier of the pairing record to save
        /// </param>
        /// <param name="recordData">
        /// buffer containing the pairing record data
        /// </param>
        /// <param name="recordSize">
        /// size of the buffer passed in recordData
        /// </param>
        /// <returns>
        /// 0 on success, a negative error value otherwise.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_save_pair_record", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int usbmuxd_save_pair_record([MarshalAsAttribute(UnmanagedType.LPStr)] string recordId, [MarshalAsAttribute(UnmanagedType.LPStr)] byte* recordData, uint recordSize);

        /// <summary>
        /// Save a pairing record
        /// </summary>
        /// <param name="recordId">
        /// the record identifier of the pairing record to save
        /// </param>
        /// <param name="recordData">
        /// buffer containing the pairing record data
        /// </param>
        /// <param name="recordSize">
        /// size of the buffer passed in recordData
        /// </param>
        /// <returns>
        /// 0 on success, a negative error value otherwise.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_save_pair_record", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int usbmuxd_save_pair_record([MarshalAsAttribute(UnmanagedType.LPStr)] string recordId, byte[] recordData, uint recordSize);

        /// <summary>
        /// Save a pairing record with device identifier
        /// </summary>
        /// <param name="recordId">
        /// the record identifier of the pairing record to save
        /// </param>
        /// <param name="deviceId">
        /// the device identifier of the connected device, or 0
        /// </param>
        /// <param name="recordData">
        /// buffer containing the pairing record data
        /// </param>
        /// <param name="recordSize">
        /// size of the buffer passed in recordData
        /// </param>
        /// <returns>
        /// 0 on success, a negative error value otherwise.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_save_pair_record_with_deviceId", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_save_pair_record_with_deviceId([MarshalAsAttribute(UnmanagedType.LPStr)] string recordId, uint deviceId, System.IntPtr recordData, uint recordSize);

        /// <summary>
        /// Save a pairing record with device identifier
        /// </summary>
        /// <param name="recordId">
        /// the record identifier of the pairing record to save
        /// </param>
        /// <param name="deviceId">
        /// the device identifier of the connected device, or 0
        /// </param>
        /// <param name="recordData">
        /// buffer containing the pairing record data
        /// </param>
        /// <param name="recordSize">
        /// size of the buffer passed in recordData
        /// </param>
        /// <returns>
        /// 0 on success, a negative error value otherwise.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_save_pair_record_with_deviceId", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int usbmuxd_save_pair_record_with_deviceId([MarshalAsAttribute(UnmanagedType.LPStr)] string recordId, uint deviceId, byte* recordData, uint recordSize);

        /// <summary>
        /// Save a pairing record with device identifier
        /// </summary>
        /// <param name="recordId">
        /// the record identifier of the pairing record to save
        /// </param>
        /// <param name="deviceId">
        /// the device identifier of the connected device, or 0
        /// </param>
        /// <param name="recordData">
        /// buffer containing the pairing record data
        /// </param>
        /// <param name="recordSize">
        /// size of the buffer passed in recordData
        /// </param>
        /// <returns>
        /// 0 on success, a negative error value otherwise.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_save_pair_record_with_deviceId", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int usbmuxd_save_pair_record_with_deviceId([MarshalAsAttribute(UnmanagedType.LPStr)] string recordId, uint deviceId, byte[] recordData, uint recordSize);

        /// <summary>
        /// Delete a pairing record
        /// </summary>
        /// <param name="recordId">
        /// the record identifier of the pairing record to delete.
        /// </param>
        /// <returns>
        /// 0 on success, a negative errno value otherwise.
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_delete_pair_record", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_delete_pair_record([MarshalAsAttribute(UnmanagedType.LPStr)] string recordId);

        /// <summary>
        /// Enable or disable the use of inotify extension. Enabled by default.
        /// Use 0 to disable and 1 to enable inotify support.
        /// This only has an effect on linux systems if inotify support has been built
        /// in. Otherwise and on all other platforms this function has no effect.
        /// </summary>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "libusbmuxd_set_use_inotify", CallingConvention = CallingConvention.Cdecl)]
        public static extern void libusbmuxd_set_use_inotify(int set);

        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "libusbmuxd_set_debug_level", CallingConvention = CallingConvention.Cdecl)]
        public static extern void libusbmuxd_set_debug_level(int level);

        /// <summary>
        /// Sets the socket type (Unix socket or TCP socket) libusbmuxd should use when connecting
        /// to usbmuxd.
        /// </summary>
        /// <param name="value">
        /// SOCKET_TYPE_UNIX or SOCKET_TYPE_TCP
        /// </param>
        /// <returns>
        /// 0 on success or negative on error
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_set_socket_type", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_set_socket_type(int value);

        /// <summary>
        /// Gets the socket type (Unix socket or TCP socket) libusbmuxd should use when connecting
        /// to usbmuxd.
        /// </summary>
        /// <param name="value">
        /// A pointer to an integer which will reveive the current socket type
        /// </param>
        /// <returns>
        /// 0 on success or negative on error
        /// </returns>
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_get_socket_type", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_get_socket_type(out UsbmuxdSocketType value);

        /// <summary>
        /// Sets the TCP endpoint to which usbmuxd will connect if the socket type is set to
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
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_set_tcp_endpoint", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_set_tcp_endpoint([MarshalAsAttribute(UnmanagedType.LPStr)] string host, ushort port);

        /// <summary>
        /// Gets the TCP endpoint to which usbmuxd will connect if th esocket type is set to
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
        [DllImportAttribute(Usbmuxd.LibraryName, EntryPoint = "usbmuxd_get_tcp_endpoint", CallingConvention = CallingConvention.Cdecl)]
        public static extern int usbmuxd_get_tcp_endpoint(out string host, out ushort port);
    }
}
#endif
