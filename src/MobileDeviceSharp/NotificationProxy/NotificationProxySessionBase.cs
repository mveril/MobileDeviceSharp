using MobileDeviceSharp.Native;
using static MobileDeviceSharp.NotificationProxy.Native.NotificationProxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using MobileDeviceSharp.NotificationProxy.Native;
#if NET7_0_OR_GREATER
using System.Runtime.InteropServices.Marshalling;
#endif
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MobileDeviceSharp.NotificationProxy
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
#if NET7_0_OR_GREATER
        private readonly GCHandle _selfHandle;
#else
        private readonly NotificationProxyNotifyCallBack? _callback;
#endif
        private SynchronizationContext _context = SynchronizationContext.Current ?? new SynchronizationContext();

        /// <summary>
        /// Initialize the service using the specified <paramref name="device"/> <paramref name="ServiceID"/> and <paramref name="withEscrowBag"/>.
        /// </summary>
        /// <param name="device">The target device</param>
        /// <param name="ServiceID">The service id</param>
        /// <param name="withEscrowBag">If <see langword="true"/> use escrowbag</param>
        public NotificationProxySessionBase(IDevice device, string ServiceID, bool withEscrowBag) : base(device, ServiceID, withEscrowBag, s_clientNewCallback)
        {
#if NET7_0_OR_GREATER
            _selfHandle = GCHandle.Alloc(this, GCHandleType.Weak);
            NotificationProxyError result;
            unsafe
            {
                result = np_set_notify_callback(Handle, &StatickCallBack, (IntPtr)_selfHandle);
            }
#else
            _callback = Callback;
            var result = np_set_notify_callback(Handle, _callback, IntPtr.Zero);
#endif
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
#if NET7_0_OR_GREATER
            _selfHandle = GCHandle.Alloc(this, GCHandleType.Weak);
            NotificationProxyError result;
            unsafe
            {
                result = np_set_notify_callback(Handle, &StatickCallBack, IntPtr.Zero);
            }
#else
            var result = np_set_notify_callback(Handle, _callback, IntPtr.Zero);
#endif
            if (result.IsError())
            {
                throw result.GetException();
            }
        }

#if NET7_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) } )]
        private unsafe static void StatickCallBack(byte* notificationNative, IntPtr userData)
        {
            var notification = Utf8StringMarshaller.ConvertToManaged(notificationNative)!;
            var _selfHandle = (GCHandle)userData;
            if (_selfHandle.Target is NotificationProxySessionBase _np)
            {
                _np.Callback(notification);
            }
        }

        private void Callback(string notification)
#else
        private void Callback(string notification, IntPtr userData)
#endif
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
