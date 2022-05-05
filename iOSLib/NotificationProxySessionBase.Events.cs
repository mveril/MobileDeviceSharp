using IOSLib.Native;
using static IOSLib.Native.NotificationProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib
{
    public abstract partial class NotificationProxySessionBase
    {
        private ConcurrentCollections.ConcurrentHashSet<string> _eventIDS = new();

        /// <summary>
        /// Run these method to define the notification we want to observe a lot of constants are available on <see cref="NotificationProxyEvents.Recevable"/>
        /// </summary>
        /// <param name="notification"></param>
        public void ObserveNotification(params string[] notifications)
        {
            foreach (var notification in notifications)
            {
                _eventIDS.Add(notification);
            }
            UpdateObservation();
        }

        private void EventCallback(string notification, IntPtr userData)
        {
            DeviceRaiseEvent(new NotificationProxyEventArgs(notification));
        }

        private void DeviceRaiseEvent(NotificationProxyEventArgs e)
        {
            NotificationProxyEvent?.Invoke(this, e);
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
