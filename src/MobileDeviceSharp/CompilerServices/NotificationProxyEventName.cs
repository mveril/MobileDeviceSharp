using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.CompilerServices
{
    /// <summary>
    /// This attribute allow to associate a .NET event to a notificationProxy event.
    /// </summary>
    [AttributeUsage(AttributeTargets.Event)]
    public sealed class NotificationProxyEventNameAttribute : Attribute
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="NotificationProxyEventNameAttribute"/> with the specified event <paramref name="name"/>.
        /// </summary>
        /// <param name="name"></param>
        public NotificationProxyEventNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the event.
        /// </summary>
        public string Name { get; set; }
    }
}
