using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Graphics;
using MobileDeviceSharp.InstallationProxy;
using MobileDeviceSharp.SpringBoardServices;

namespace MobileDeviceSharp.SpringBoardServices
{
    /// <summary>
    /// Defines a static class containing extention methods for the <see cref="Application"/> class that avoid direct usage of <see cref="SpringBoardServicesSession"/> for SpringBoard related properties like icon.
    /// </summary>
    public static class ApplicationExtension
    {
        /// <summary>
        /// Retrieves the icon for the specified <paramref name="application"/> using a <see cref="SpringBoardServicesSession"/>.
        /// </summary>
        /// <param name="application">The <see cref="Application"/> instance for which to retrieve the icon.</param>
        /// <returns>An <see cref="IImage"/> representing the application icon.</returns>
        public static IImage GetIcon(this Application application)
        {
            using var sbservice = new SpringBoardServicesSession(application.Device);
            return sbservice.GetIcon(application);
        }
    }
}
