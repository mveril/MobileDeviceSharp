using IOSLib.Native;
using static IOSLib.NotificationProxy.Native.NotificationProxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using IOSLib.NotificationProxy.Native;

namespace IOSLib.NotificationProxy
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
        private readonly NotificationProxyNotifyCallBack? _callback;
        private SynchronizationContext _context = SynchronizationContext.Current ?? new SynchronizationContext();

        /// <summary>
        /// Initialize the service using the specified <paramref name="device"/> <paramref name="ServiceID"/> and <paramref name="withEscrowBag"/>.
        /// </summary>
        /// <param name="device">The target device</param>
        /// <param name="ServiceID">The service id</param>
        /// <param name="withEscrowBag">If <see langword="true"/> use escrowbag</param>
        public NotificationProxySessionBase(IDevice device, string ServiceID, bool withEscrowBag) : base(device, ServiceID, withEscrowBag, s_clientNewCallback)
        {
            _callback = Callback;
            var result = np_set_notify_callback(Handle, _callback, IntPtr.Zero);
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
            _callback = Callback;
            var result = np_set_notify_callback(Handle, _callback, IntPtr.Zero);
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
