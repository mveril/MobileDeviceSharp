using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib
{
    /// <summary>
    /// Class that contains event argument for <see cref="NotificationProxySession"/> events
    /// </summary>
    public class NotificationProxyEventArgs : EventArgs
    {
        /// <summary>
        /// The name of the notificationproxy event. Lot of constants are available <see cref="IOSLib.Native.NotificationProxyEvents"/>.
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
