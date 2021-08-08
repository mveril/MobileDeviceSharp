using IOSLib.Native;
using static IOSLib.Native.NotificationProxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IOSLib
{
    public abstract class NotificationProxySessionBase : ServiceSessionBase<NotificationProxyClientHandle,NotificationProxyError>
    {
        public event NotificationProxyEventHandler? NotificationProxyEvent;
        public NotificationProxySessionBase(IDevice device, string ServiceID, bool withEscrowBag) : base(device, ServiceID, withEscrowBag,new ClientNewCallback<NotificationProxyClientHandle, NotificationProxyError>(np_client_new))
        {
            
        }

        public NotificationProxySessionBase(IDevice device) : base(device, new StartServiceCallback<NotificationProxyClientHandle, NotificationProxyError>(np_client_start_service))
        {

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
