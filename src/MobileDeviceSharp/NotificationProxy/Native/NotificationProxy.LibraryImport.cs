﻿#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using MobileDeviceSharp.CompilerServices;
using MobileDeviceSharp.Native;

namespace MobileDeviceSharp.NotificationProxy.Native
{
    internal static partial class NotificationProxy
    {
        /// <summary>
        /// Connects to the notification_proxy on the specified device.
        /// </summary>
        /// <param name="device">
        /// The device to connect to.
        /// </param>
        /// <param name="service">
        /// The service descriptor returned by lockdownd_start_service.
        /// </param>
        /// <param name="client">
        /// Pointer that will be set to a newly allocated np_client_t
        /// upon successful return.
        /// </param>
        /// <returns>
        /// NP_E_SUCCESS on success, NP_E_INVALID_ARG when device is NULL,
        /// or NP_E_CONN_FAILED when the connection to the device could not be
        /// established.
        /// </returns>
        [LibraryImportAttribute(NotificationProxy.LibraryName, EntryPoint = "np_client_new")]
        public static partial NotificationProxyError np_client_new(IDeviceHandle device, LockdownServiceDescriptorHandle service, out NotificationProxyClientHandle client);

        /// <summary>
        /// Starts a new notification proxy service on the specified device and connects to it.
        /// </summary>
        /// <param name="device">
        /// The device to connect to.
        /// </param>
        /// <param name="client">
        /// Pointer that will point to a newly allocated
        /// np_client_t upon successful return. Must be freed using
        /// np_client_free() after use.
        /// </param>
        /// <param name="label">
        /// The label to use for communication. Usually the program name.
        /// Pass NULL to disable sending the label in requests to lockdownd.
        /// </param>
        /// <returns>
        /// NP_E_SUCCESS on success, or an NP_E_* error
        /// code otherwise.
        /// </returns>
        [LibraryImportAttribute(NotificationProxy.LibraryName, EntryPoint = "np_client_start_service")]
        public static partial NotificationProxyError np_client_start_service(IDeviceHandle device, out NotificationProxyClientHandle client, [MarshalAsAttribute(UnmanagedType.LPStr)] string? label);

        /// <summary>
        /// Disconnects a notification_proxy client from the device and frees up the
        /// notification_proxy client data.
        /// </summary>
        /// <param name="client">
        /// The notification_proxy client to disconnect and free.
        /// </param>
        /// <returns>
        /// NP_E_SUCCESS on success, or NP_E_INVALID_ARG when client is NULL.
        /// </returns>
        [UsedForRelease<NotificationProxyClientHandle>]
        [LibraryImportAttribute(NotificationProxy.LibraryName, EntryPoint = "np_client_free")]
        public static partial NotificationProxyError np_client_free(System.IntPtr client);

        /// <summary>
        /// Sends a notification to the device's notification_proxy.
        /// </summary>
        /// <param name="client">
        /// The client to send to
        /// </param>
        /// <param name="notification">
        /// The notification message to send
        /// </param>
        /// <returns>
        /// NP_E_SUCCESS on success, or an error returned by np_plist_send
        /// </returns>
        [LibraryImportAttribute(NotificationProxy.LibraryName, EntryPoint = "np_post_notification")]
        public static partial NotificationProxyError np_post_notification(NotificationProxyClientHandle client, [MarshalAsAttribute(UnmanagedType.LPStr)] string notification);

        /// <summary>
        /// Tells the device to send a notification on the specified event.
        /// </summary>
        /// <param name="client">
        /// The client to send to
        /// </param>
        /// <param name="notification">
        /// The notifications that should be observed.
        /// </param>
        /// <returns>
        /// NP_E_SUCCESS on success, NP_E_INVALID_ARG when client or
        /// notification are NULL, or an error returned by np_plist_send.
        /// </returns>
        [LibraryImportAttribute(NotificationProxy.LibraryName, EntryPoint = "np_observe_notification")]
        public static partial NotificationProxyError np_observe_notification(NotificationProxyClientHandle client, [MarshalAsAttribute(UnmanagedType.LPStr)] string notification);

        /// <summary>
        /// Tells the device to send a notification on specified events.
        /// </summary>
        /// <param name="client">
        /// The client to send to
        /// </param>
        /// <param name="notificationSpec">
        /// Specification of the notifications that should be
        /// observed. This is expected to be an array of const char* that MUST have a
        /// terminating NULL entry.
        /// </param>
        /// <returns>
        /// NP_E_SUCCESS on success, NP_E_INVALID_ARG when client is null,
        /// or an error returned by np_observe_notification.
        /// </returns>
        [LibraryImportAttribute(NotificationProxy.LibraryName, EntryPoint = "np_observe_notifications", StringMarshalling = StringMarshalling.Utf8)]
        public static partial NotificationProxyError np_observe_notifications(NotificationProxyClientHandle client, [MarshalUsing(typeof(NullTerminatedArrayMarshaller<string, IntPtr>), ConstantElementCount = -1)] string[] notificationSpec);

        /// <summary>
        /// This function allows an application to define a callback function that will
        /// be called when a notification has been received.
        /// It will start a thread that polls for notifications and calls the callback
        /// function if a notification has been received.
        /// In case of an error condition when polling for notifications - e.g. device
        /// disconnect - the thread will call the callback function with an empty
        /// notification "" and terminate itself.
        /// </summary>
        /// <param name="client">
        /// the NP client
        /// </param>
        /// <param name="notifyCallBack">
        /// pointer to a callback function or NULL to de-register a
        /// previously set callback function.
        /// </param>
        /// <param name="userData">
        /// Pointer that will be passed to the callback function as
        /// user data. If notify_cb is NULL, this parameter is ignored.
        /// </param>
        /// <returns>
        /// NP_E_SUCCESS when the callback was successfully registered,
        /// NP_E_INVALID_ARG when client is NULL, or NP_E_UNKNOWN_ERROR when
        /// the callback thread could no be created.
        /// </returns>
        /// <remarks>
        /// Only one callback function can be registered at the same time;
        /// any previously set callback function will be removed automatically.
        /// </remarks>
        [LibraryImportAttribute(NotificationProxy.LibraryName, EntryPoint = "np_set_notify_callback")]
        public static partial NotificationProxyError np_set_notify_callback(NotificationProxyClientHandle client, NotificationProxyNotifyCallBack notifyCallBack, System.IntPtr userData);
    }
}
#endif
