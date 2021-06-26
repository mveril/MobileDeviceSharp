using System;
using System.Collections.Generic;
using System.Text;
using IOSLib;
using IOSLib.Native;

namespace IOSLib
{
    class NotificationProxy : NotificationProxyBase
    {
        const string serviceID = "com.apple.mobile.notification_proxy";

       public NotificationProxy(IDevice device) : base(device, serviceID) { }

    }
}
