using IOSLib.Native;
using IOSLib.NotificationProxy.Native;
using static IOSLib.NotificationProxy.Native.NotificationProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib.NotificationProxy
{
    public abstract partial class NotificationProxySessionBase
    {
        private ConcurrentCollections.ConcurrentHashSet<string> _eventIDS = new();

        /// <summary>
        /// Define the notification we want to observe. A lot of constants are available on <see cref="NotificationProxyEvents.Recevable"/>
        /// </summary>
        /// <param name="notifications"></param>
        public void ObserveNotification(params string[] notifications)
        {
            var result = np_observe_notifications(Handle, notifications);
            if (result.IsError())
            {
                throw result.GetException();
            }
            else
            {
                foreach (var notification in notifications)
                {
                    _eventIDS.Add(notification);
                }
            }
        }

        /// <summary>
        /// Define the notification we want to observe. A lot of constants are available on <see cref="NotificationProxyEvents.Recevable"/>
        /// </summary>
        /// <param name="notification"></param>
        public void ObserveNotification(string notification)
        {
            var result = np_observe_notification(Handle, notification);
            if (result.IsError())
            {
                throw result.GetException();
            }
            else
            {
                _eventIDS.Add(notification);
            }
        }

        private void EventCallback(string notification)
        {
            _context.Post(_ => DeviceRaiseEvent(new NotificationProxyEventArgs(notification)), null);            
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
            RaiseEvent(e.EventName);
        }
    }
}
