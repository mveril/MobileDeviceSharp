using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Graphics.Skia;
using MobileDeviceSharp.Native;
using MobileDeviceSharp.PropertyList;
using MobileDeviceSharp.SpringBoardServices.Native;
using MobileDeviceSharp.InstallationProxy;
using static MobileDeviceSharp.SpringBoardServices.Native.SpringBoardServices;

namespace MobileDeviceSharp.SpringBoardServices
{
    /// <summary>
    /// Represent a session of the libimobiledevice <see href="https://docs.libimobiledevice.org/libimobiledevice/latest/sbservices_8h.html">SpringBoardServices</see>.
    /// </summary>
    public class SpringBoardServicesSession : ServiceSessionBase<SpringBoardServicesClientHandle, SpringBoardServicesError>
    {
        private static StartServiceCallback<SpringBoardServicesClientHandle, SpringBoardServicesError> s_startService = sbservices_client_start_service;

        /// <summary>
        /// Initialize a new instance of <see cref="SpringBoardServicesSession"/> for the specific <paramref name="device"/>.
        /// </summary>
        /// <param name="device">The target device.</param>
        public SpringBoardServicesSession(IDevice device) : base(device, s_startService)
        {

        }

        /// <summary>
        /// Get the icon of the specified <paramref name="application"/>.
        /// </summary>
        /// <param name="application"></param>
        /// <returns>The <see cref="IImage"/> representing the icon.</returns>
        public IImage GetIcon(Application application)
        {
            return GetIcon(application.BundleID);
        }

        /// <summary>
        /// Get the icon of the application with the specified <paramref name="bundleId"/>.
        /// </summary>
        /// <param name="bundleId">The bundle identifier of the applicaition.</param>
        /// <returns>The <see cref="IImage"/> representing the icon.</returns>
        public IImage GetIcon(string bundleId)
        {
            var hresult = sbservices_get_icon_pngdata(Handle, bundleId, out var pngData, out _);
            if (hresult.IsError())
                throw hresult.GetException();
            var ms = new MemoryStream(pngData);
            var img = SkiaImage.FromStream(ms);
            return img;
        }
        /// <summary>
        /// Get the wallpaper image of the device.
        /// </summary>
        /// <param name="bundleId">The bundle identifier of the applicaition.</param>
        /// <returns>The <see cref="IImage"/> of the wallpaper.</returns>
        public IImage GetWallpaper()
        {
            var hresult = sbservices_get_home_screen_wallpaper_pngdata(Handle, out var pngData, out _);
            if (hresult.IsError())
                throw hresult.GetException();
            var ms = new MemoryStream(pngData);
            var img = SkiaImage.FromStream(ms);
            return img;
        }

        /// <summary>
        /// Get the SpringBoard disposition of the device for the requiested version.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public SpringBoardDissposition GetDisposition(string version)
        {
            var hresult = sbservices_get_icon_state(Handle, out var state, version);
            if (hresult.IsError())
                throw hresult.GetException();
            var array = (PlistArray)PlistNode.From(state)!;
            return new SpringBoardDissposition(array, version);
        }

        /// <summary>
        /// Apply the specified SpringBoard dissposition.
        /// </summary>
        /// <param name="springBoardDissposition"></param>
        public void SetDisposition(SpringBoardDissposition springBoardDissposition)
        {
            SetDisposition(springBoardDissposition.Disposition);
        }

        /// <summary>
        /// Apply the specified SpringBoard dissposition.
        /// </summary>
        /// <param name="springBoardDissposition">A <see cref="PlistNode"/> containing the disposition.</param>
        public void SetDisposition(PlistNode springBoardDissposition)
        {
            var hresult = sbservices_set_icon_state(Handle, springBoardDissposition.Handle);
            if (hresult.IsError())
                throw hresult.GetException();
        }

        /// <summary>
        /// Get the interface orientation of the device.
        /// </summary>
        public UIInterfaceOrientation Orientation
        {
            get
            {
                sbservices_get_interface_orientation(Handle, out var orientation);
                return orientation;
            }
        }
    }
}
