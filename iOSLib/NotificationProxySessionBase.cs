using IOSLib.Native;
using static IOSLib.Native.NotificationProxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace IOSLib
{
    /// <summary>
    /// Represente a session of the <see href="https://docs.libimobiledevice.org/libimobiledevice/latest/notification__proxy_8h.html">NotificationProxy</see> service.
    /// </summary>
    public abstract partial class NotificationProxySessionBase : ServiceSessionBase<NotificationProxyClientHandle,NotificationProxyError>
    {
        /// <summary>
        /// Event fired when a subcribed event occurs.
        /// </summary>
        public event NotificationProxyEventHandler? NotificationProxyEvent;

        private static readonly StartServiceCallback<NotificationProxyClientHandle, NotificationProxyError> s_startCallback = np_client_start_service;

        private static readonly ClientNewCallback<NotificationProxyClientHandle, NotificationProxyError> s_clientNewCallback = np_client_new;
        private SynchronizationContext _context = new SynchronizationContext();

        /// <summary>
        /// Initialize the service using the specified <paramref name="device"/> <paramref name="ServiceID"/> and <paramref name="withEscrowBag"/>.
        /// </summary>
        /// <param name="device">The target device</param>
        /// <param name="ServiceID">The service id</param>
        /// <param name="withEscrowBag">If <see langword="true"/> use escrowbag</param>
        public NotificationProxySessionBase(IDevice device, string ServiceID, bool withEscrowBag) : base(device, ServiceID, withEscrowBag, s_clientNewCallback)
        {
            var result = np_set_notify_callback(Handle, Callback, IntPtr.Zero);
            if (result.IsError())
            {
                throw result.GetException();
            }
        }

        /// <summary>
        /// Initialize the service using the specified <paramref name="device"/>.
        /// </summary>
        /// <param name="device"></param>
        public NotificationProxySessionBase(IDevice device) : base(device, s_startCallback)
        {
            var result = np_set_notify_callback(Handle, Callback, IntPtr.Zero);
            if (result.IsError())
            {
                throw result.GetException();
            }
        }

        private void Callback(string notification, IntPtr userData)
        {
            EventCallback(notification);
            TaskCallBack(notification);       
        }

        /// <summary>
        /// Raise event to the device <see cref="NotificationProxyEvents.Sendable"/>
        /// </summary>
        /// <param name="eventName">The event args</param>
        public void RaiseEvent(string eventName)
        {
            np_post_notification(Handle, eventName);
        }
    }
}
