#if !NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MobileDeviceSharp.CompilerServices;
using MobileDeviceSharp.Native;
using MobileDeviceSharp.PropertyList.Native;

namespace MobileDeviceSharp.SpringBoardServices.Native
{
    internal static partial class SpringBoardServices
    {
        /// <summary>
        /// Connects to the springboardservices service on the specified device.
        /// </summary>
        /// <param name="device">
        /// The device to connect to.
        /// </param>
        /// <param name="service">
        /// The service descriptor returned by lockdownd_start_service.
        /// </param>
        /// <param name="client">
        /// Pointer that will point to a newly allocated
        /// sbservices_client_t upon successful return.
        /// </param>
        /// <returns>
        /// SBSERVICES_E_SUCCESS on success, SBSERVICES_E_INVALID_ARG when
        /// client is NULL, or an SBSERVICES_E_* error code otherwise.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(SpringBoardServices.LibraryName, EntryPoint = "sbservices_client_new", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
            public static extern SpringBoardServicesError sbservices_client_new(IDeviceHandle device, LockdownServiceDescriptorHandle service, out SpringBoardServicesClientHandle client);

            /// <summary>
            /// Starts a new sbservices service on the specified device and connects to it.
            /// </summary>
            /// <param name="device">
            /// The device to connect to.
            /// </param>
            /// <param name="client">
            /// Pointer that will point to a newly allocated
            /// sbservices_client_t upon successful return. Must be freed using
            /// sbservices_client_free() after use.
            /// </param>
            /// <param name="label">
            /// The label to use for communication. Usually the program name.
            /// Pass NULL to disable sending the label in requests to lockdownd.
            /// </param>
            /// <returns>
            /// SBSERVICES_E_SUCCESS on success, or an SBSERVICES_E_* error
            /// code otherwise.
            /// </returns>
            [System.Runtime.InteropServices.DllImportAttribute(SpringBoardServices.LibraryName, EntryPoint = "sbservices_client_start_service", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
            public static extern SpringBoardServicesError sbservices_client_start_service(IDeviceHandle device, out SpringBoardServicesClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string label);

            /// <summary>
            /// Disconnects an sbservices client from the device and frees up the
            /// sbservices client data.
            /// </summary>
            /// <param name="client">
            /// The sbservices client to disconnect and free.
            /// </param>
            /// <returns>
            /// SBSERVICES_E_SUCCESS on success, SBSERVICES_E_INVALID_ARG when
            /// client is NULL, or an SBSERVICES_E_* error code otherwise.
            /// </returns>
            [UsedForRelease<SpringBoardServicesClientHandle>]
            [System.Runtime.InteropServices.DllImportAttribute(SpringBoardServices.LibraryName, EntryPoint = "sbservices_client_free", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
            public static extern SpringBoardServicesError sbservices_client_free(System.IntPtr client);

            /// <summary>
            /// Gets the icon state of the connected device.
            /// </summary>
            /// <param name="client">
            /// The connected sbservices client to use.
            /// </param>
            /// <param name="state">
            /// Pointer that will point to a newly allocated plist containing
            /// the current icon state. It is up to the caller to free the memory.
            /// </param>
            /// <param name="formatVersion">
            /// A string to be passed as formatVersion along with
            /// the request, or NULL if no formatVersion should be passed. This is only
            /// supported since iOS 4.0 so for older firmware versions this must be set
            /// to NULL.
            /// </param>
            /// <returns>
            /// SBSERVICES_E_SUCCESS on success, SBSERVICES_E_INVALID_ARG when
            /// client or state is invalid, or an SBSERVICES_E_* error code otherwise.
            /// </returns>
            [System.Runtime.InteropServices.DllImportAttribute(SpringBoardServices.LibraryName, EntryPoint = "sbservices_get_icon_state", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
            public static extern SpringBoardServicesError sbservices_get_icon_state(SpringBoardServicesClientHandle client, out PlistHandle state, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string formatVersion);

            /// <summary>
            /// Sets the icon state of the connected device.
            /// </summary>
            /// <param name="client">
            /// The connected sbservices client to use.
            /// </param>
            /// <param name="newstate">
            /// A plist containing the new iconstate.
            /// </param>
            /// <returns>
            /// SBSERVICES_E_SUCCESS on success, SBSERVICES_E_INVALID_ARG when
            /// client or newstate is NULL, or an SBSERVICES_E_* error code otherwise.
            /// </returns>
            [System.Runtime.InteropServices.DllImportAttribute(SpringBoardServices.LibraryName, EntryPoint = "sbservices_set_icon_state", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
            public static extern SpringBoardServicesError sbservices_set_icon_state(SpringBoardServicesClientHandle client, PlistHandle newstate);

            /// <summary>
            /// Get the icon of the specified app as PNG data.
            /// </summary>
            /// <param name="client">
            /// The connected sbservices client to use.
            /// </param>
            /// <param name="bundleId">
            /// The bundle identifier of the app to retrieve the icon for.
            /// </param>
            /// <param name="pngData">
            /// Pointer that will point to a newly allocated buffer
            /// containing the PNG data upon successful return. It is up to the caller
            /// to free the memory.
            /// </param>
            /// <param name="pngSize">
            /// Pointer to a uint64_t that will be set to the size of the
            /// buffer pngdata points to upon successful return.
            /// </param>
            /// <returns>
            /// SBSERVICES_E_SUCCESS on success, SBSERVICES_E_INVALID_ARG when
            /// client, bundleId, or pngdata are invalid, or an SBSERVICES_E_* error
            /// code otherwise.
            /// </returns>
            [System.Runtime.InteropServices.DllImportAttribute(SpringBoardServices.LibraryName, EntryPoint = "sbservices_get_icon_pngdata", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
            public static extern SpringBoardServicesError sbservices_get_icon_pngdata(SpringBoardServicesClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string bundleId, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] out byte[] pngData, out ulong pngSize);

            /// <summary>
            /// Gets the interface orientation of the device.
            /// </summary>
            /// <param name="client">
            /// The connected sbservices client to use.
            /// </param>
            /// <param name="interfaceOrientation">
            /// The interface orientation upon successful return.
            /// </param>
            /// <returns>
            /// SBSERVICES_E_SUCCESS on success, SBSERVICES_E_INVALID_ARG when
            /// client or state is invalid, or an SBSERVICES_E_* error code otherwise.
            /// </returns>
            [System.Runtime.InteropServices.DllImportAttribute(SpringBoardServices.LibraryName, EntryPoint = "sbservices_get_interface_orientation", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
            public static extern SpringBoardServicesError sbservices_get_interface_orientation(SpringBoardServicesClientHandle client, out UIInterfaceOrientation interfaceOrientation);

            /// <summary>
            /// Get the home screen wallpaper as PNG data.
            /// </summary>
            /// <param name="client">
            /// The connected sbservices client to use.
            /// </param>
            /// <param name="pngData">
            /// Pointer that will point to a newly allocated buffer
            /// containing the PNG data upon successful return. It is up to the caller
            /// to free the memory.
            /// </param>
            /// <param name="pngSize">
            /// Pointer to a uint64_t that will be set to the size of the
            /// buffer pngdata points to upon successful return.
            /// </param>
            /// <returns>
            /// SBSERVICES_E_SUCCESS on success, SBSERVICES_E_INVALID_ARG when
            /// client or pngdata are invalid, or an SBSERVICES_E_* error
            /// code otherwise.
            /// </returns>
            [System.Runtime.InteropServices.DllImportAttribute(SpringBoardServices.LibraryName, EntryPoint = "sbservices_get_home_screen_wallpaper_pngdata", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
            public static extern SpringBoardServicesError sbservices_get_home_screen_wallpaper_pngdata(SpringBoardServicesClientHandle client, [MarshalAs(UnmanagedType.LPArray,SizeParamIndex =2)] out byte[] pngData, out ulong pngSize);
    }
}
#endif
