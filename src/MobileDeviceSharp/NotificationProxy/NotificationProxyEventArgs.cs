using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.NotificationProxy
{
    /// <summary>
    /// Class that contains event argument for <see cref="NotificationProxySession"/> events
    /// </summary>
    public sealed class NotificationProxyEventArgs : EventArgs
    {
        /// <summary>
        /// The name of the notificationproxy event. Lot of constants are available <see cref="Native.NotificationProxyEvents"/>.
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Create the event args.
        /// </summary>
        /// <param name="name">The event name</param>
        public NotificationProxyEventArgs(string name) : base()
        {
            EventName = name;
        }
    }
}
