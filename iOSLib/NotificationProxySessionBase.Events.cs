﻿using IOSLib.Native;
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
        /// Run these method to define the notification we want to observe a lot of constants are available on <see cref="NotificationProxyEvents.Recevable"/>
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
            RaiseEvent(e.EventName);
        }
    }
}
