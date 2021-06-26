using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib
{
    public class NotificationProxyEventArgs : EventArgs
    {
        public string EventName { get; }
        
        public NotificationProxyEventArgs(string Name) : base()
        {
            EventName = Name;
        }
    }
}
