using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MobileDeviceSharp;
using MobileDeviceSharp.InstallationProxy.Native;
using MobileDeviceSharp.Native;
using static MobileDeviceSharp.InstallationProxy.Native.InstallationProxy;
using MobileDeviceSharp.PropertyList;
using MobileDeviceSharp.PropertyList.Native;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
#if NETCOREAPP3_0_OR_GREATER
using System.Threading.Channels;
#endif

namespace MobileDeviceSharp.InstallationProxy
{
    /// <summary>
    /// Represente a session of the <see href="https://docs.libimobiledevice.org/libimobiledevice/latest/installation__proxy_8h.html">InstallationProxy</see> service.
    /// </summary>
    public class InstallationProxySession : ServiceSessionBase<InstallationProxyClientHandle, InstallationProxyError>
    {
        private static readonly StartServiceCallback<InstallationProxyClientHandle, InstallationProxyError> s_startService = instproxy_client_start_service;
        
        /// <summary>
        /// Create an installation proxy session for the specified <paramref name="device"/>.
        /// </summary>
        /// <param name="device"></param>
        public InstallationProxySession(IDevice device) : base(device, s_startService)
        {

        }

        private static bool AppVisibleOrDispose(Application app)
        {
            bool isVisible;
            try
            {
                isVisible = app.IsVisible;
            }
            catch (Exception)
            {
                app?.Dispose();
                throw;
            }
            if (!isVisible)
            {
                app?.Dispose();
            }
            return isVisible;
        }

        /// <summary>
        /// Get the list of application for the device.
        /// </summary>
        /// <param name="options">Option used to filter the list.</param>
        /// <param name="showHidden">Indicate wether the hidden apps should be listed.</param>
        /// <returns>The list of applications.</returns>
        public IEnumerable<Application> GetApplications(InstalltionProxyLookupOptions? options, bool showHidden)
        {
            using var optDic = options?.ToDictionary();
            var hresult = instproxy_browse(Handle, optDic?.Handle ?? PlistHandle.Zero, out var plistHandle);
            if (hresult.IsError())
                throw hresult.GetException();
            using var appsPlist = (PlistArray)PlistNode.From(plistHandle)!;
            foreach (var item in appsPlist)
            {
                var app = new Application(Device, (PlistDictionary)item.Clone());
                if (showHidden || AppVisibleOrDispose(app))
                {
                    yield return app;
                }
            }
        }

        /// <summary>
        /// Get the list of application for the device.
        /// </summary>
        /// <param name="options">Option used to filter the list.</param>
        /// <returns>The list of applications.</returns>
        public IEnumerable<Application> GetApplications(InstalltionProxyLookupOptions options)
        {
            return GetApplications(options, false);
        }

        public IEnumerable<Application> GetApplications(bool showHidden)
        {
            return GetApplications(null, showHidden);
        }

        /// <summary>
        /// Get the list of application for the device.
        /// </summary>
        /// <returns>The list of applications.</returns>
        public IEnumerable<Application> GetApplications()
        {
            return GetApplications(null, false);
        }

        private PlistDictionary LookupInternal(string[] bundleIds)
        {
            var hresult = instproxy_lookup(Handle, bundleIds, PlistHandle.Zero, out var result);
            if (hresult.IsError())
                throw hresult.GetException();
            return (PlistDictionary)PlistNode.From(result)!;
        }

        /// <summary>
        /// Get application by Id.
        /// The selected application.
        /// </summary>
        /// <param name="bundleId">A value corresponding to the <see cref="Application.BundleID"/></param>
        public Application GetApplication(string bundleId)
        {
            var bundleIds = new string[] { bundleId };
            using PlistDictionary dic = LookupInternal(bundleIds);
            return new Application(Device, (PlistDictionary)dic[bundleId].Clone());
        }

        /// <summary>
        /// Get applications by Ids.
        /// </summary>
        /// <param name="bundleIds">A the list of <see cref="Application.BundleID"/> of the Applications we target.</param>
        /// <returns>A readonly dictionary containing <see cref="Application.BundleID"/> as key and <see cref="Application"/> as values</returns>
        public IReadOnlyDictionary<string, Application> GetApplications(IEnumerable<string> bundleIds)
        {
            return GetApplications(bundleIds.ToArray());
        }

        /// <summary>
        /// Get applications by Ids.
        /// </summary>
        /// <param name="bundleIds">A the list of <see cref="Application.BundleID"/> of the Applications we target.</param>
        /// <returns>A readonly dictionary containing <see cref="Application.BundleID"/> as key and <see cref="Application"/> as values</returns>
        public IReadOnlyDictionary<string, Application> GetApplications(params string[] bundleIds)
        {
            using var dic = LookupInternal(bundleIds);
            var appsDic = new Dictionary<string, Application>();
            foreach (var item in dic)
            {
                appsDic.Add(item.Key, new Application(Device, (PlistDictionary)item.Value.Clone()));
            };
            return new ReadOnlyDictionary<string, Application>(appsDic);
        }



#if NETCOREAPP3_0_OR_GREATER

        /// <summary>
        /// Get the list of application for the device asynchroniously.
        /// </summary>
        /// <param name="options">Option used to filter the list.</param>
        /// <param name="showHidden">Indicate wether the hidden apps should be listed.</param>
        /// <returns>The list of applications.</returns>
        public async IAsyncEnumerable<Application> GetApplicationsAsync(InstalltionProxyLookupOptions? options, bool showHidden)
        {
            await foreach (var item in GetApplicationsCoreAsync(options))
            {
                var app = new Application(Device, (PlistDictionary)item);
                if(showHidden || AppVisibleOrDispose(app)){
                    yield return app;
                }
            }
        }


        IAsyncEnumerable<PlistNode> GetApplicationsCoreAsync(InstalltionProxyLookupOptions? options)
        {
            var channel = Channel.CreateUnbounded<PlistNode>(new UnboundedChannelOptions() { SingleWriter = true, SingleReader = true });
            var gchandle = GCHandle.Alloc(new EnumerateOperationStatusContext(channel.Writer));
            var ptr = GCHandle.ToIntPtr(gchandle);
            using var dic = options?.ToDictionary();
#if NET7_0_OR_GREATER
            InstallationProxyError hresult;
            unsafe
            {
                hresult = instproxy_browse_with_callback(Handle, dic?.Handle ?? PlistHandle.Zero, &OperationStatusCallbackNative, ptr);
            }
#else
            var hresult = instproxy_browse_with_callback(Handle, dic?.Handle ?? PlistHandle.Zero, s_operationStatusCallback, ptr);
#endif
            if (hresult.IsError())
                throw hresult.GetException();
            return channel.Reader.ReadAllAsync();
        }

        /// <summary>
        /// Get the list of application for the device asynchroniously.
        /// </summary>
        /// <param name="options">Option used to filter the list.</param>
        /// <returns>The list of applications.</returns>
        public IAsyncEnumerable<Application> GetApplicationsAsync(InstalltionProxyLookupOptions options)
        {
            return GetApplicationsAsync(options, false);
        }

        /// <summary>
        /// Get the list of application for the device asynchroniously.
        /// </summary>
        /// <param name="showHidden">Indicate wether the hidden apps should be listed.</param>
        /// <returns>The list of applications.</returns>
        public IAsyncEnumerable<Application> GetApplicationsAsync(bool showHidden)
        {
            return GetApplicationsAsync(null, showHidden);
        }

        /// <summary>
        /// Get the list of application for the device asynchroniously.
        /// </summary>
        /// <returns>The list of applications.</returns>
        public IAsyncEnumerable<Application> GetApplicationsAsync()
        {
            return GetApplicationsAsync(null, false);
        }
#endif
        /// <summary>
        /// Check if the selected <paramref name="capabilities"/>  are avalable on the device.
        /// </summary>
        /// <param name="capabilities">The capabilities to check.</param>
        /// <returns>A <see cref="CapabilityMatcher"/>Object used to store the results</returns>
        public CapabilityMatcher CheckCapabilityMatch(params string[] capabilities)
        {
            return CheckCapabilityMatch(capabilities.AsEnumerable());
        }

        /// <summary>
        /// Check if the selected <paramref name="capabilities"/>  are avalable on the device.
        /// </summary>
        /// <param name="capabilities">The capabilities to check.</param>
        /// <returns>A <see cref="CapabilityMatcher"/>Object used to store the results</returns>
        public CapabilityMatcher CheckCapabilityMatch(IEnumerable<string> capabilities)
        {
            return CheckCapabilityMatch(new HashSet<string>(capabilities));
        }

        /// <summary>
        /// Check if the selected <paramref name="capabilities"/>  are avalable on the device.
        /// </summary>
        /// <param name="capabilities">The capabilities to check.</param>
        /// <returns>A <see cref="CapabilityMatcher"/>Object used to store the results</returns>
#if NET5_0_OR_GREATER
        public CapabilityMatcher CheckCapabilityMatch(IReadOnlySet<string> capabilities)
#else
        public CapabilityMatcher CheckCapabilityMatch(ISet<string> capabilities)
#endif
        {
            var hresult = instproxy_check_capabilities_match(Handle, capabilities.ToArray(), PlistHandle.Zero, out var plistMatchHandle);
            if (hresult.IsError())
                throw hresult.GetException();
            var plistMatch = (PlistDictionary)PlistNode.From(plistMatchHandle)!;
            var match = ((PlistBoolean)plistMatch["CapabilitiesMatch"]).Value;
            
            if(plistMatch.TryGetValue("MismatchedCapabilities",out var plistMismatch) && plistMismatch is PlistArray plistMismatchArray)
            {
                var caps=plistMismatchArray.Select(node => ((PlistString)node).Value);
                return new CapabilityMatcher(match, capabilities, new HashSet<string>(caps));
            }
            return new CapabilityMatcher(match, capabilities, new HashSet<string>());
        }

#if !NET7_0_OR_GREATER
        private static readonly InstallationProxyStatusCallBack s_operationStatusCallback = OperationStatusCallback;
#endif

#if NET7_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void OperationStatusCallbackNative(IntPtr command, IntPtr status, IntPtr userData)
        {
            var commandPlist = new PlistNotOwnedHandle();
            Marshal.InitHandle(commandPlist, command);
            var statusPlist = new PlistNotOwnedHandle();
            Marshal.InitHandle(statusPlist, status);
            OperationStatusCallback(commandPlist, statusPlist, userData);
        }
#endif

        private static void OperationStatusCallback(PlistNotOwnedHandle command, PlistNotOwnedHandle status, IntPtr userData)
        {
            var gchandle = GCHandle.FromIntPtr(userData);
            var context = (OperationStatusContext)gchandle.Target!;
            context.ReportProgress(command, status);
            if (OperationStatusContext.IsComplete(status))
            {
                gchandle.Free();
            }
        }

        /// <summary>
        /// Install the application stored in the computer.
        /// </summary>
        /// <param name="path">The path of the .ipa file to install.</param>
        /// <returns></returns>
        public Task InstallAsync(string path)
        {
            return InstallAsync(path, null, null);
        }

        /// <summary>
        /// Install an application using a .ipa file stored on the computer.
        /// </summary>
        /// <param name="path">The path of the .ipa file to install.</param>
        /// <param name="options">Options used to specify the way of installing.</param>
        /// <returns></returns>
        public Task InstallAsync(string path, InstallationProxyInstallOptions? options)
        {
            return InstallAsync(path, options, null);
        }

        /// <summary>
        /// Install an application using a .ipa file stored on the computer.
        /// </summary>
        /// <param name="path">The path of the .ipa file to install.</param>
        /// <param name="progress">A <see cref="IProgress{Int32}"/> used to report the progress percentage.</param>
        /// <returns></returns>
        public Task InstallAsync(string path, IProgress<int>? progress)
        {
            return InstallAsync(path, null, progress);
        }

        /// <summary>
        /// Install an application using a .ipa file stored on the computer.
        /// </summary>
        /// <param name="path">The path of the .ipa file to install.</param>
        /// <param name="options">Options used to specify the way of installing.</param>
        /// <param name="progress">A <see cref="IProgress{Int32}"/> used to report the progress percentage.</param>
        /// <returns></returns>
        public Task InstallAsync(string path, InstallationProxyInstallOptions? options, IProgress<int>? progress)
        {
            using var optDic = options?.ToDictionary();
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            var handle = GCHandle.Alloc(new TaskWithProgressOperationStatusContext(tcs, progress));
#if NET7_0_OR_GREATER
            unsafe
            {
                instproxy_install(Handle, path, optDic?.Handle ?? PlistHandle.Zero, &OperationStatusCallbackNative, GCHandle.ToIntPtr(handle));
            }
#else
            instproxy_install(Handle, path, optDic?.Handle ?? PlistHandle.Zero, s_operationStatusCallback, GCHandle.ToIntPtr(handle));
#endif
            return tcs.Task;
        }

        /// <summary>
        /// Upgrade an application using a .ipa file stored on the computer.
        /// </summary>
        /// <param name="path">The path of the .ipa file to install.</param>
        /// <returns></returns>
        public Task UpgradeAsync(string path)
        {
            return UpgradeAsync(path, null, null);
        }

        /// <summary>
        /// Upgrade an application using a .ipa file stored on the computer.
        /// </summary>
        /// <param name="path">The path of the .ipa file to install.</param>
        /// <param name="progress">An <see cref="IProgress{Int32}"/> used to report the progress percentage.</param>
        /// <returns></returns>
        public Task UpgradeAsync(string path, IProgress<int>? progress)
        {
            return UpgradeAsync(path, null, progress);
        }

        /// <summary>
        /// Upgrade an application using a .ipa file stored on the computer.
        /// </summary>
        /// <param name="path">The path of the .ipa file to install.</param>
        /// <param name="options">Options used to specify the way of installing.</param>
        /// <param name="progress">An <see cref="IProgress{Int32}"/> used to report the progress percentage.</param>
        /// <returns></returns>
        public Task UpgradeAsync(string path, InstallationProxyInstallOptions? options, IProgress<int>? progress)
        {
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            var handle = GCHandle.Alloc(new TaskWithProgressOperationStatusContext(tcs, progress));
#if NET7_0
            unsafe
            {
                instproxy_upgrade(Handle, path, PlistHandle.Zero, &OperationStatusCallbackNative, GCHandle.ToIntPtr(handle));
            }
#else
            instproxy_upgrade(Handle, path, PlistHandle.Zero, s_operationStatusCallback, GCHandle.ToIntPtr(handle));
#endif
            return tcs.Task;
        }

        /// <summary>
        /// Uninstall the application with the specified <paramref name="bundleId"/>.
        /// </summary>
        /// <param name="bundleId">The Bundle identifier of the application to uninstall.</param>
        /// <returns></returns>
        public Task UninstallAsync(string bundleId)
        {
            return UninstallAsync(bundleId, null);
        }

        /// <summary>
        /// Uninstall the application with the specified <paramref name="bundleId"/>.
        /// </summary>
        /// <param name="bundleId">The Bundle identifier of the application to uninstall.</param>
        /// <param name="progress">An <see cref="IProgress{Int32}"/> used to report the progress percentage.</param>
        /// <returns></returns>
        public Task UninstallAsync(string bundleId, IProgress<int>? progress)
        {
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            var handle = GCHandle.Alloc(new TaskWithProgressOperationStatusContext(tcs, progress));
#if NET7_0_OR_GREATER
            unsafe
            {
                instproxy_archive(Handle, bundleId, PlistHandle.Zero, &OperationStatusCallbackNative, GCHandle.ToIntPtr(handle));;
            }
#else
            instproxy_archive(Handle, bundleId, PlistHandle.Zero, s_operationStatusCallback, GCHandle.ToIntPtr(handle));
#endif
            return tcs.Task;
        }

        /// <summary>
        /// Uninstall the application with the specified <paramref name="bundleId"/>.
        /// </summary>
        /// <param name="bundleId">The Bundle identifier of the application to archive.</param>
        /// <param name="options">Options used to specify the way of archiving.</param>
        /// <returns></returns>
        public Task ArchiveAsync(string bundleId, InstallationProxyArchiveOptions? options)
        {
            return ArchiveAsync(bundleId, options, null);
        }

        /// <summary>
        /// Uninstall the application with the specified <paramref name="bundleId"/>.
        /// </summary>
        /// <param name="bundleId">The Bundle identifier of the application to archive.</param>
        /// <param name="options">Options used to specify the way of archiving.</param>
        /// <param name="progress">An <see cref="IProgress{Int32}"/> used to report the progress percentage.</param>
        /// <returns></returns>
        public Task ArchiveAsync(string bundleId, InstallationProxyArchiveOptions? options, IProgress<int>? progress)
        {
            using var optDic = options?.ToDictionary();
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            var handle = GCHandle.Alloc(new TaskWithProgressOperationStatusContext(tcs, progress));
#if NET7_0_OR_GREATER
            unsafe
            {
                instproxy_archive(Handle, bundleId, optDic?.Handle ?? PlistHandle.Zero, &OperationStatusCallbackNative, GCHandle.ToIntPtr(handle));
            }
#else
            instproxy_archive(Handle, bundleId, optDic?.Handle ?? PlistHandle.Zero, s_operationStatusCallback, GCHandle.ToIntPtr(handle));
#endif
            return tcs.Task;
        }

        /// <summary>
        /// Uninstall the application with the specified <paramref name="bundleId"/>.
        /// </summary>
        /// <param name="bundleId">The Bundle identifier of the application to archive.</param>
        /// <param name="progress">An <see cref="IProgress{Int32}"/> used to report the progress percentage.</param>
        /// <returns></returns>
        public Task ArchiveAsync(string bundleId, IProgress<int>? progress)
        {
            return ArchiveAsync(bundleId, null, progress);
        }

        /// <summary>
        /// Uninstall the application with the specified <paramref name="bundleId"/>.
        /// </summary>
        /// <param name="bundleId">The Bundle identifier of the application to archive.</param>
        /// <returns></returns>
        public Task ArchiveAsync(string bundleId)
        {
            return ArchiveAsync(bundleId, null, null);
        }

        /// <summary>
        /// Restore the application with the specified <paramref name="bundleId"/>.
        /// </summary>
        /// <param name="bundleId">The Bundle identifier of the application to restore.</param>
        /// <returns></returns>
        public Task RestoreAsync(string bundleId)
        {
            return RestoreAsync(bundleId, null);
        }

        /// <summary>
        /// Restore the application with the specified <paramref name="bundleId"/>.
        /// </summary>
        /// <param name="bundleId">The Bundle identifier of the application to restore.</param>
        /// <param name="progress">An <see cref="IProgress{Int32}"/> used to report the progress percentage.</param>
        /// <returns></returns>
        public Task RestoreAsync(string bundleId, IProgress<int>? progress)
        {
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            var handle = GCHandle.Alloc(new TaskWithProgressOperationStatusContext(tcs, progress));
#if NET7_0_OR_GREATER
            unsafe
            {
                instproxy_restore(Handle, bundleId, PlistHandle.Zero, &OperationStatusCallbackNative, GCHandle.ToIntPtr(handle)); ;
            }
#else
            instproxy_restore(Handle, bundleId, PlistHandle.Zero, s_operationStatusCallback, GCHandle.ToIntPtr(handle)); ;
#endif
            return tcs.Task;
        }
    }
}
