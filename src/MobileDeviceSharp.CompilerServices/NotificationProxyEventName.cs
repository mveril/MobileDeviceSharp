using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.CompilerServices
{
    [AttributeUsage(AttributeTargets.Event)]
    public class NotificationProxyEventNameAttribute : Attribute
    {
        public NotificationProxyEventNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
