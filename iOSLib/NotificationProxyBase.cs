using IOSLib.Native;
using static IOSLib.Native.NotificationProxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IOSLib
{
    public abstract class NotificationProxyBase : IDisposable
    {
        public event NotificationProxyEventHandler? NotificationProxyEvent;
        public NotificationProxyBase(IDevice device, string ServiceID)
        {
            LockdownServiceDescriptorHandle descriptorHandle;
            using (var lickdown = new Lockdown(device))
            {
                descriptorHandle = lickdown.StartService(ServiceID);
            }
            var ex = np_client_new(device.Handle,descriptorHandle, out var handle).GetException();
            if (ex != null)
            {
                throw ex;
            }
            Handle = handle;
        }

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

        public void RaiseEvent(NotificationProxyEventArgs e)
        {
            np_post_notification(Handle, e.EventName);
            NotificationProxyEvent?.Invoke(this, e);
        }

        public NotificationProxyClientHandle Handle { get; } = NotificationProxyClientHandle.Zero;

        public void Dispose()
        {
            ((IDisposable)Handle).Dispose();
        }
    }
}
