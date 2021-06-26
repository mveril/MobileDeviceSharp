using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib
{
    public class InsecureNotificationProxy : NotificationProxyBase
    {
        const string serviceID = "com.apple.mobile.insecure_notification_proxy";
        public InsecureNotificationProxy(IDevice device) : base(device, serviceID) { } 
    }
}
