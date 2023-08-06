#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.CompilerServices;
using System.Runtime.InteropServices;
using MobileDeviceSharp.PropertyList;
using MobileDeviceSharp.PropertyList.Native;
using MobileDeviceSharp.InstallationProxy;
using MobileDeviceSharp.Native;
using System.Runtime.InteropServices.Marshalling;

namespace MobileDeviceSharp.InstallationProxy.Native
{
    internal static partial class InstallationProxy
    {
        /// <summary>
        /// Connects to the installation_proxy service on the specified device.
        /// </summary>
        /// <param name="device">
        /// The device to connect to
        /// </param>
        /// <param name="service">
        /// The service descriptor returned by lockdownd_start_service.
        /// </param>
        /// <param name="client">
        /// Pointer that will be set to a newly allocated
        /// instproxy_client_t upon successful return.
        /// </param>
        /// <returns>
        /// INSTPROXY_E_SUCCESS on success, or an INSTPROXY_E_* error value
        /// when an error occurred.
        /// </returns>
        [System.Runtime.InteropServices.LibraryImport(InstallationProxy.LibraryName, EntryPoint = "instproxy_client_new")]
        public static partial InstallationProxyError instproxy_client_new(IDeviceHandle device, LockdownServiceDescriptorHandle service, out InstallationProxyClientHandle client);

        /// <summary>
        /// Starts a new installation_proxy service on the specified device and connects to it.
        /// </summary>
        /// <param name="device">
        /// The device to connect to.
        /// </param>
        /// <param name="client">
        /// Pointer that will point to a newly allocated
        /// instproxy_client_t upon successful return. Must be freed using
        /// instproxy_client_free() after use.
        /// </param>
        /// <param name="label">
        /// The label to use for communication. Usually the program name.
        /// Pass NULL to disable sending the label in requests to lockdownd.
        /// </param>
        /// <returns>
        /// INSTPROXY_E_SUCCESS on success, or an INSTPROXY_E_* error
        /// code otherwise.
        /// </returns>
        [System.Runtime.InteropServices.LibraryImport(InstallationProxy.LibraryName, EntryPoint = "instproxy_client_start_service")]
        public static partial InstallationProxyError instproxy_client_start_service(IDeviceHandle device, out InstallationProxyClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string label);

        /// <summary>
        /// Disconnects an installation_proxy client from the device and frees up the
        /// installation_proxy client data.
        /// </summary>
        /// <param name="client">
        /// The installation_proxy client to disconnect and free.
        /// </param>
        /// <returns>
        /// INSTPROXY_E_SUCCESS on success
        /// or INSTPROXY_E_INVALID_ARG if client is NULL.
        /// </returns>
        [UsedForRelease<InstallationProxyClientHandle>]
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_client_free")]
        public static partial InstallationProxyError instproxy_client_free(System.IntPtr client);

        /// <summary>
        /// List installed applications. This function runs synchronously.
        /// </summary>
        /// <param name="client">
        /// The connected installation_proxy client
        /// </param>
        /// <param name="clientOptions">
        /// The client options to use, as PLIST_DICT, or NULL.
        /// Valid client options include:
        /// "ApplicationType" -> "System"
        /// "ApplicationType" -> "User"
        /// "ApplicationType" -> "Internal"
        /// "ApplicationType" -> "Any"
        /// </param>
        /// <param name="result">
        /// Pointer that will be set to a plist that will hold an array
        /// of PLIST_DICT holding information about the applications found.
        /// </param>
        /// <returns>
        /// INSTPROXY_E_SUCCESS on success or an INSTPROXY_E_* error value if
        /// an error occurred.
        /// </returns>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_browse")]
        public static partial InstallationProxyError instproxy_browse(InstallationProxyClientHandle client, PlistHandle clientOptions, out PlistHandle result);

        /// <summary>
        /// List pages of installed applications in a callback.
        /// </summary>
        /// <param name="client">
        /// The connected installation_proxy client
        /// </param>
        /// <param name="clientOptions">
        /// The client options to use, as PLIST_DICT, or NULL.
        /// Valid client options include:
        /// "ApplicationType" -> "System"
        /// "ApplicationType" -> "User"
        /// "ApplicationType" -> "Internal"
        /// "ApplicationType" -> "Any"
        /// </param>
        /// <param name="statusCallBack">
        /// Callback function to process each page of application
        /// information. Passing a callback is required.
        /// </param>
        /// <param name="userData">
        /// Callback data passed to statusCallBack.
        /// </param>
        /// <returns>
        /// INSTPROXY_E_SUCCESS on success or an INSTPROXY_E_* error value if
        /// an error occurred.
        /// </returns>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_browse_with_callback")]
        public static unsafe partial InstallationProxyError instproxy_browse_with_callback(InstallationProxyClientHandle client, PlistHandle clientOptions, delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr,void> statusCallBack, System.IntPtr userData);

        /// <summary>
        /// Lookup information about specific applications from the device.
        /// </summary>
        /// <param name="client">
        /// The connected installation_proxy client
        /// </param>
        /// <param name="appids">
        /// An array of bundle identifiers that MUST have a terminating
        /// NULL entry or NULL to lookup all.
        /// </param>
        /// <param name="clientOptions">
        /// The client options to use, as PLIST_DICT, or NULL.
        /// Currently there are no known client options, so pass NULL here.
        /// </param>
        /// <param name="result">
        /// Pointer that will be set to a plist containing a PLIST_DICT
        /// holding requested information about the application or NULL on errors.
        /// </param>
        /// <returns>
        /// INSTPROXY_E_SUCCESS on success or an INSTPROXY_E_* error value if
        /// an error occurred.
        /// </returns>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_lookup")]
        public static partial InstallationProxyError instproxy_lookup(InstallationProxyClientHandle client, [MarshalUsing(typeof(NullTerminatedUTF8StringArrayMarshaller))] string[] appids, PlistHandle clientOptions, out PlistHandle result);

        /// <summary>
        /// Install an application on the device.
        /// </summary>
        /// <param name="client">
        /// The connected installation_proxy client
        /// </param>
        /// <param name="pkgPath">
        /// Path of the installation package (inside the AFC jail)
        /// </param>
        /// <param name="clientOptions">
        /// The client options to use, as PLIST_DICT, or NULL.
        /// Valid options include:
        /// "iTunesMetadata" -> PLIST_DATA
        /// "ApplicationSINF" -> PLIST_DATA
        /// "PackageType" -> "Developer"
        /// If PackageType -> Developer is specified, then pkgPath points to
        /// an .app directory instead of an install package.
        /// </param>
        /// <param name="statusCallBack">
        /// Callback function for progress and status information. If
        /// NULL is passed, this function will run synchronously.
        /// </param>
        /// <param name="userData">
        /// Callback data passed to statusCallBack.
        /// </param>
        /// <returns>
        /// INSTPROXY_E_SUCCESS on success or an INSTPROXY_E_* error value if
        /// an error occurred.
        /// </returns>
        /// <remarks>
        /// If a callback function is given (async mode), this function returns
        /// INSTPROXY_E_SUCCESS immediately if the status updater thread has been
        /// created successfully; any error occuring during the command has to be
        /// handled inside the specified callback function.
        /// </remarks>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_install")]
        public static unsafe partial InstallationProxyError instproxy_install(InstallationProxyClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pkgPath, PlistHandle clientOptions, delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr, void> statusCallBack, System.IntPtr userData);

        /// <summary>
        /// Upgrade an application on the device. This function is nearly the same as
        /// instproxy_install; the difference is that the installation progress on the
        /// device is faster if the application is already installed.
        /// </summary>
        /// <param name="client">
        /// The connected installation_proxy client
        /// </param>
        /// <param name="pkgPath">
        /// Path of the installation package (inside the AFC jail)
        /// </param>
        /// <param name="clientOptions">
        /// The client options to use, as PLIST_DICT, or NULL.
        /// Valid options include:
        /// "iTunesMetadata" -> PLIST_DATA
        /// "ApplicationSINF" -> PLIST_DATA
        /// "PackageType" -> "Developer"
        /// If PackageType -> Developer is specified, then pkgPath points to
        /// an .app directory instead of an install package.
        /// </param>
        /// <param name="statusCallBack">
        /// Callback function for progress and status information. If
        /// NULL is passed, this function will run synchronously.
        /// </param>
        /// <param name="userData">
        /// Callback data passed to statusCallBack.
        /// </param>
        /// <returns>
        /// INSTPROXY_E_SUCCESS on success or an INSTPROXY_E_* error value if
        /// an error occurred.
        /// </returns>
        /// <remarks>
        /// If a callback function is given (async mode), this function returns
        /// INSTPROXY_E_SUCCESS immediately if the status updater thread has been
        /// created successfully; any error occuring during the command has to be
        /// handled inside the specified callback function.
        /// </remarks>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_upgrade")]
        public static unsafe partial InstallationProxyError instproxy_upgrade(InstallationProxyClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string pkgPath, PlistHandle clientOptions, delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr, void> statusCallBack, System.IntPtr userData);

        /// <summary>
        /// Uninstall an application from the device.
        /// </summary>
        /// <param name="client">
        /// The connected installation proxy client
        /// </param>
        /// <param name="appid">
        /// ApplicationIdentifier of the app to uninstall
        /// </param>
        /// <param name="clientOptions">
        /// The client options to use, as PLIST_DICT, or NULL.
        /// Currently there are no known client options, so pass NULL here.
        /// </param>
        /// <param name="statusCallBack">
        /// Callback function for progress and status information. If
        /// NULL is passed, this function will run synchronously.
        /// </param>
        /// <param name="userData">
        /// Callback data passed to statusCallBack.
        /// </param>
        /// <returns>
        /// INSTPROXY_E_SUCCESS on success or an INSTPROXY_E_* error value if
        /// an error occurred.
        /// </returns>
        /// <remarks>
        /// If a callback function is given (async mode), this function returns
        /// INSTPROXY_E_SUCCESS immediately if the status updater thread has been
        /// created successfully; any error occuring during the command has to be
        /// handled inside the specified callback function.
        /// </remarks>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_uninstall")]
        public static unsafe partial InstallationProxyError instproxy_uninstall(InstallationProxyClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string appid, PlistHandle clientOptions, delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr, void> statusCallBack, System.IntPtr userData);

        /// <summary>
        /// List archived applications. This function runs synchronously.
        /// </summary>
        /// <param name="client">
        /// The connected installation_proxy client
        /// </param>
        /// <param name="clientOptions">
        /// The client options to use, as PLIST_DICT, or NULL.
        /// Currently there are no known client options, so pass NULL here.
        /// </param>
        /// <param name="result">
        /// Pointer that will be set to a plist containing a PLIST_DICT
        /// holding information about the archived applications found.
        /// </param>
        /// <returns>
        /// INSTPROXY_E_SUCCESS on success or an INSTPROXY_E_* error value if
        /// an error occurred.
        /// </returns>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_lookup_archives")]
        public static partial InstallationProxyError instproxy_lookup_archives(InstallationProxyClientHandle client, PlistHandle clientOptions, out PlistHandle result);

        /// <summary>
        /// Archive an application on the device.
        /// This function tells the device to make an archive of the specified
        /// application. This results in the device creating a ZIP archive in the
        /// 'ApplicationArchives' directory and uninstalling the application.
        /// </summary>
        /// <param name="client">
        /// The connected installation proxy client
        /// </param>
        /// <param name="appid">
        /// ApplicationIdentifier of the app to archive.
        /// </param>
        /// <param name="clientOptions">
        /// The client options to use, as PLIST_DICT, or NULL.
        /// Valid options include:
        /// "SkipUninstall" -> Boolean
        /// "ArchiveType" -> "ApplicationOnly"
        /// </param>
        /// <param name="statusCallBack">
        /// Callback function for progress and status information. If
        /// NULL is passed, this function will run synchronously.
        /// </param>
        /// <param name="userData">
        /// Callback data passed to statusCallBack.
        /// </param>
        /// <returns>
        /// INSTPROXY_E_SUCCESS on success or an INSTPROXY_E_* error value if
        /// an error occurred.
        /// </returns>
        /// <remarks>
        /// If a callback function is given (async mode), this function returns
        /// INSTPROXY_E_SUCCESS immediately if the status updater thread has been
        /// created successfully; any error occuring during the command has to be
        /// handled inside the specified callback function.
        /// </remarks>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_archive")]
        public static unsafe partial InstallationProxyError instproxy_archive(InstallationProxyClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string appid, PlistHandle clientOptions, delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr, void> statusCallBack, System.IntPtr userData);

        /// <summary>
        /// Restore a previously archived application on the device.
        /// This function is the counterpart to instproxy_archive.
        /// </summary>
        /// <param name="client">
        /// The connected installation proxy client
        /// </param>
        /// <param name="appid">
        /// ApplicationIdentifier of the app to restore.
        /// </param>
        /// <param name="clientOptions">
        /// The client options to use, as PLIST_DICT, or NULL.
        /// Valid options include:
        /// "ArchiveType" -> "DocumentsOnly"
        /// </param>
        /// <param name="statusCallBack">
        /// Callback function for progress and status information. If
        /// NULL is passed, this function will run synchronously.
        /// </param>
        /// <param name="userData">
        /// Callback data passed to statusCallBack.
        /// </param>
        /// <returns>
        /// INSTPROXY_E_SUCCESS on success or an INSTPROXY_E_* error value if
        /// an error occurred.
        /// </returns>
        /// <remarks>
        /// If a callback function is given (async mode), this function returns
        /// INSTPROXY_E_SUCCESS immediately if the status updater thread has been
        /// created successfully; any error occuring during the command has to be
        /// handled inside the specified callback function.
        /// </remarks>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_restore")]
        public static unsafe partial InstallationProxyError instproxy_restore(InstallationProxyClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string appid, PlistHandle clientOptions, delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr, void> statusCallBack, System.IntPtr userData);


        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_check_capabilities_match")]
        public static partial InstallationProxyError instproxy_check_capabilities_match(InstallationProxyClientHandle client, [MarshalUsing(typeof(NullTerminatedUTF8StringArrayMarshaller))] string[] capabilities, PlistHandle clientOptions, out PlistHandle result);

        /// <summary>
        /// Checks a device for certain capabilities.
        /// </summary>
        /// <param name="client">
        /// The connected installation_proxy client
        /// </param>
        /// <param name="capabilities">
        /// An array of char* with capability names that MUST have a
        /// terminating NULL entry.
        /// </param>
        /// <param name="clientOptions">
        /// The client options to use, as PLIST_DICT, or NULL.
        /// Currently there are no known client options, so pass NULL here.
        /// </param>
        /// <param name="result">
        /// Pointer that will be set to a plist containing a PLIST_DICT
        /// holding information if the capabilities matched or NULL on errors.
        /// </param>
        /// <returns>
        /// INSTPROXY_E_SUCCESS on success or an INSTPROXY_E_* error value if
        /// an error occurred.
        /// </returns>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_check_capabilities_match")]
        public static partial InstallationProxyError instproxy_check_capabilities_match(InstallationProxyClientHandle client, [MarshalUsing(typeof(NullTerminatedUTF8StringArrayMarshaller))] out  string[] capabilities, PlistHandle clientOptions, out PlistHandle result);

        /// <summary>
        /// Gets the name from a command dictionary.
        /// </summary>
        /// <param name="command">
        /// The dictionary describing the command.
        /// </param>
        /// <param name="name">
        /// Pointer to store the name of the command.
        /// </param>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_command_get_name", StringMarshalling=StringMarshalling.Utf8)]
        public static partial void instproxy_command_get_name(PlistHandle command, out string name);

        /// <summary>
        /// Gets the name of a status.
        /// </summary>
        /// <param name="status">
        /// The dictionary status response to use.
        /// </param>
        /// <param name="name">
        /// Pointer to store the name of the status.
        /// </param>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_status_get_name", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void instproxy_status_get_name(PlistHandle status, out string name);

        /// <summary>
        /// Gets error name, code and description from a response if available.
        /// </summary>
        /// <param name="status">
        /// The dictionary status response to use.
        /// </param>
        /// <param name="name">
        /// Pointer to store the name of an error.
        /// </param>
        /// <param name="description">
        /// Pointer to store error description text if available.
        /// The caller is reponsible for freeing the allocated buffer after use.
        /// If NULL is passed no description will be returned.
        /// </param>
        /// <param name="code">
        /// Pointer to store the returned error code if available.
        /// If NULL is passed no error code will be returned.
        /// </param>
        /// <returns>
        /// INSTPROXY_E_SUCCESS if no error is found or an INSTPROXY_E_* error
        /// value matching the error that ẃas found in the status.
        /// </returns>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_status_get_error", StringMarshalling = StringMarshalling.Utf8)]
        public static partial InstallationProxyError instproxy_status_get_error(PlistHandle status, out string name, out string description, out ulong code);

        /// <summary>
        /// Gets total and current item information from a browse response if available.
        /// </summary>
        /// <param name="status">
        /// The dictionary status response to use.
        /// </param>
        /// <param name="total">
        /// Pointer to store the total number of items.
        /// </param>
        /// <param name="currentIndex">
        /// Pointer to store the current index of all browsed items.
        /// </param>
        /// <param name="currentAmount">
        /// Pointer to store the amount of items in the
        /// current list.
        /// </param>
        /// <param name="list">
        /// Pointer to store a newly allocated plist with items.
        /// The caller is reponsible for freeing the list after use.
        /// If NULL is passed no list will be returned. If NULL is returned no
        /// list was found in the status.
        /// </param>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_status_get_current_list")]
        public static partial void instproxy_status_get_current_list(PlistHandle status, out ulong total, out ulong currentIndex, out ulong currentAmount, out PlistHandle list);

        /// <summary>
        /// Gets progress in percentage from a status if available.
        /// </summary>
        /// <param name="status">
        /// The dictionary status response to use.
        /// </param>
        /// <param name="percent">
        /// Pointer to store the progress in percent (0-100) or -1 if not
        /// progress was found in the status.
        /// </param>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_status_get_percent_complete")]
        public static partial void instproxy_status_get_percent_complete(PlistHandle status, out int percent);

        /// <summary>
        /// Creates a new clientOptions plist.
        /// </summary>
        /// <returns>
        /// A new plist_t of type PLIST_DICT.
        /// </returns>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_clientOptions_new")]
        public static partial PlistHandle instproxy_clientOptions_new();

        /// <summary>
        /// Adds one or more new key:value pairs to the given clientOptions.
        /// </summary>
        /// <param name="clientOptions">
        /// The client options to modify.
        /// </param>
        /// <param name="...">
        /// KEY, VALUE, [KEY, VALUE], NULL
        /// </param>
        /// <remarks>
        /// The keys and values passed are expected to be strings, except for the
        /// keys "ApplicationSINF", "iTunesMetadata", "ReturnAttributes" which are
        /// expecting a plist_t node as value and "SkipUninstall" expects int.
        /// </remarks>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_clientOptions_add")]
        public static partial void instproxy_clientOptions_add(PlistHandle clientOptions);

        /// <summary>
        /// Adds attributes to the given clientOptions to filter browse results.
        /// </summary>
        /// <param name="clientOptions">
        /// The client options to modify.
        /// </param>
        /// <param name="...">
        /// VALUE, VALUE, [VALUE], NULL
        /// </param>
        /// <remarks>
        /// The values passed are expected to be strings.
        /// </remarks>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_clientOptions_set_return_attributes")]
        public static partial void instproxy_clientOptions_set_return_attributes(PlistHandle clientOptions);

        /// <summary>
        /// Frees clientOptions plist.
        /// </summary>
        /// <param name="clientOptions">
        /// The client options plist to free. Does nothing if NULL
        /// is passed.
        /// </param>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_clientOptions_free")]
        public static partial void instproxy_clientOptions_free(PlistHandle clientOptions);

        /// <summary>
        /// Queries the device for the path of an application.
        /// </summary>
        /// <param name="client">
        /// The connected installation proxy client.
        /// </param>
        /// <param name="appId">
        /// ApplicationIdentifier of app to retrieve the path for.
        /// </param>
        /// <param name="path">
        /// Pointer to store the device path for the application
        /// which is set to NULL if it could not be determined.
        /// </param>
        /// <returns>
        /// INSTPROXY_E_SUCCESS on success, INSTPROXY_E_OP_FAILED if
        /// the path could not be determined or an INSTPROXY_E_* error
        /// value if an error occurred.
        /// </returns>
        [System.Runtime.InteropServices.LibraryImportAttribute(InstallationProxy.LibraryName, EntryPoint = "instproxy_client_get_path_for_bundle_identifier")]
        public static partial InstallationProxyError instproxy_client_get_path_for_bundle_identifier(InstallationProxyClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string appId, out System.IntPtr path);
    }
}
#endif
