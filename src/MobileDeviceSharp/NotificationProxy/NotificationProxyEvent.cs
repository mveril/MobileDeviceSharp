using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.NotificationProxy
{
    /// <summary>
    /// Represent the handler for NotificationProxy events
    /// </summary>
    /// <param name="sender">The sender, Normaly an object that herits from <see cref="NotificationProxySessionBase"/></param>.
    /// <param name="e">The event args.</param>
    public delegate void NotificationProxyEventHandler(object sender, NotificationProxyEventArgs e);
}
