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
        /// <summary>
        /// Initialize the service using the specified <paramref name="device"/> <paramref name="ServiceID"/> and <paramref name="withEscrowBag"/>.
        /// </summary>
        /// <param name="device">The target device</param>
        /// <param name="ServiceID">The service id</param>
        /// <param name="withEscrowBag">If <see langword="true"/> use escrowbag</param>
        public NotificationProxySessionBase(IDevice device, string ServiceID, bool withEscrowBag) : base(device, ServiceID, withEscrowBag, s_clientNewCallback)
        {
            var ex = np_set_notify_callback(Handle, Callback, IntPtr.Zero).GetException();
            if (ex != null)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Initialize the service using the specified <paramref name="device"/>.
        /// </summary>
        /// <param name="device"></param>
        public NotificationProxySessionBase(IDevice device) : base(device, s_startCallback)
        {
            var ex = np_set_notify_callback(Handle, Callback, IntPtr.Zero).GetException();
            if (ex != null)
            {
                throw ex;
            }
        }

        private void UpdateObservation()
        {
            np_observe_notifications(this.Handle, _tasksDic.Keys.Concat(_eventIDS).ToArray());
        }

        private void Callback(string notification, IntPtr userData)
        {
            TaskCallBack(notification, userData);
            EventCallback(notification, userData);
        }
    }
}
