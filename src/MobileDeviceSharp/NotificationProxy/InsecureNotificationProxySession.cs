﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.NotificationProxy
{
    /// <summary>
    /// An insecure version of the <see cref="NotificationProxySession"/> to use when the device is not pared
    /// </summary>
    public sealed class InsecureNotificationProxySession : NotificationProxySessionBase
    {
        const string ServiceID = "com.apple.mobile.insecure_notification_proxy";

        /// <inheritdoc/>
        public InsecureNotificationProxySession(IDevice device) : base(device, ServiceID, false) { }
    }
}
