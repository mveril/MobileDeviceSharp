using IOSLib.Native;
using static IOSLib.Native.NotificationProxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IOSLib
{
    /// <summary>
    /// Represente a session of the <see href="https://docs.libimobiledevice.org/libimobiledevice/latest/notification__proxy_8h.html">NotificationProxy</see> service.
    /// </summary>
    public abstract class NotificationProxySessionBase : ServiceSessionBase<NotificationProxyClientHandle,NotificationProxyError>
    {
        /// <summary>
        /// Event fired when a subcribed event occurs.
        /// </summary>
        public event NotificationProxyEventHandler? NotificationProxyEvent;

        private static readonly StartServiceCallback<NotificationProxyClientHandle, NotificationProxyError> startCallback = np_client_start_service;

        private static readonly ClientNewCallback<NotificationProxyClientHandle, NotificationProxyError> clientNewCallback = np_client_new ;
        /// <summary>
        /// Initialize the service using the specified <paramref name="device"/> <paramref name="ServiceID"/> and <paramref name="withEscrowBag"/>.
        /// </summary>
        /// <param name="device">The target device</param>
        /// <param name="ServiceID">The service id</param>
        /// <param name="withEscrowBag">If <see langword="true"/> use escrowbag</param>
        public NotificationProxySessionBase(IDevice device, string ServiceID, bool withEscrowBag) : base(device, ServiceID, withEscrowBag,new ClientNewCallback<NotificationProxyClientHandle, NotificationProxyError>(np_client_new))
        {
            
        }

        /// <summary>
        /// Initialize the service using the specified <paramref name="device"/>.
        /// </summary>
        /// <param name="device"></param>
        public NotificationProxySessionBase(IDevice device) : base(device, startCallback)
        {

        }
        /// <summary>
        /// Run these method to define the notification we want to observe a lot of constants are available on <see cref="NotificationProxyEvents.Recevable"/>
        /// </summary>
        /// <param name="notification"></param>
        public void ObserveNotification(params string[] notification)
        {
            var ex = np_observe_notifications(Handle, notification).GetException();
            if (ex != null)
            {
                throw ex;
            }
            ex = np_set_notify_callback(Handle, callBack, IntPtr.Zero).GetException();
            if (ex != null)
            {
                throw ex;
            }
        }

        private void callBack(string notification, IntPtr userData)
        {
            DeviceRaiseEvent(new NotificationProxyEventArgs(notification));
        }

        private void DeviceRaiseEvent(NotificationProxyEventArgs e)
        {
            NotificationProxyEvent?.Invoke(this ,e);
        }

        /// <summary>
        /// Raise event to the device <see cref="NotificationProxyEvents.Sendable"/>
        /// </summary>
        /// <param name="e">The event args</param>
        public void RaiseEvent(NotificationProxyEventArgs e)
        {
            np_post_notification(Handle, e.EventName);
            NotificationProxyEvent?.Invoke(this, e);
        }
    }
}
