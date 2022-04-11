using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib
{
    /// <summary>
    /// An insecure version of the <see cref="NotificationProxySession"/> to use when the device is not pared
    /// </summary>
    public class InsecureNotificationProxySession : NotificationProxySessionBase
    {
        const string serviceID = "com.apple.mobile.insecure_notification_proxy";

        /// <inheritdoc/>
        public InsecureNotificationProxySession(IDevice device) : base(device, serviceID, false) { } 
    }
}
