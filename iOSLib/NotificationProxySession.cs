using System;
using System.Collections.Generic;
using System.Text;
using IOSLib;
using IOSLib.Native;

namespace IOSLib
{
    class NotificationProxySession : NotificationProxySessionBase
    {
        const string serviceID = "com.apple.mobile.notification_proxy";

       public NotificationProxySession(IDevice device) : base(device, serviceID) 
        { 
        
        }

    }
}
