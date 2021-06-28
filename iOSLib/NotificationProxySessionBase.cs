using IOSLib.Native;
using static IOSLib.Native.NotificationProxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IOSLib
{
    public abstract class NotificationProxySessionBase : ServiceSessionBase<NotificationProxyClientHandle>
    {
        public event NotificationProxyEventHandler? NotificationProxyEvent;
        public NotificationProxySessionBase(IDevice device, string ServiceID, bool withEscrowBag) : base(device, ServiceID, withEscrowBag)
        {
            
        }

        public NotificationProxySessionBase(IDevice device) : base(device)
        {

        }

        protected override NotificationProxyClientHandle Init()
        {
            var ex = np_client_start_service(Device.Handle, out var handle, null).GetException();
            if (ex != null)
            {
                throw ex;
            }
            return handle;
        }

        protected override NotificationProxyClientHandle Init(LockdownServiceDescriptorHandle Descriptor)
        {
            var ex = np_client_new(Device.Handle, Descriptor, out var handle).GetException();
            if (ex != null)
            {
                throw ex;
            }
            return Handle;
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
    }
}
