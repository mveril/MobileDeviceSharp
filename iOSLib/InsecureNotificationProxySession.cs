using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib
{
    public class InsecureNotificationProxySession : NotificationProxySessionBase
    {
        const string serviceID = "com.apple.mobile.insecure_notification_proxy";
        public InsecureNotificationProxySession(IDevice device) : base(device, serviceID, false) { } 
    }
}
