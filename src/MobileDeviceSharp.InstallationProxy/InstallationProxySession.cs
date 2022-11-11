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
#if NETCOREAPP3_0_OR_GREATER
using System.Threading.Channels;
#endif

namespace MobileDeviceSharp.InstallationProxy
{
    public class InstallationProxySession : ServiceSessionBase<InstallationProxyClientHandle, InstallationProxyError>
    {
        private static readonly StartServiceCallback<InstallationProxyClientHandle, InstallationProxyError> s_startService = instproxy_client_start_service;
        public InstallationProxySession(IDevice device) : base(device, s_startService)
        {

        }

        public IEnumerable<Application> GetApplications(InstalltionProxyLookupOptions? options, bool showHidden)
        {
            using var optDic = options?.ToDictionary();
            var hresult = instproxy_browse(Handle, optDic.Handle ?? PlistHandle.Zero, out var plistHandle);
            if (hresult.IsError())
                throw hresult.GetException();
            using var appsPlist = (PlistArray)PlistNode.From(plistHandle)!;
            var apps = appsPlist.Select((item) => new Application(Device, (PlistDictionary)item.Clone()));
            if (showHidden)
            {
                return apps;
            }
            return apps.Where((app) => app.IsVisible);
        }

        public IEnumerable<Application> GetApplications(InstalltionProxyLookupOptions options)
        {
            return GetApplications(options, false);
        }

        public IEnumerable<Application> GetApplications(bool showHidden)
        {
            return GetApplications(null, showHidden);
        }

        public IEnumerable<Application> GetApplications()
        {
            return GetApplications(null, false);
        }

#if NETCOREAPP3_0_OR_GREATER
        public IAsyncEnumerable<Application> GetApplicationsAsync(InstalltionProxyLookupOptions? options, bool showHidden)
        {
            var channel = Channel.CreateUnbounded<Application>(new UnboundedChannelOptions() { SingleWriter = true, SingleReader = true });
            var callback = new InstallationProxyStatusCallBack(Callback);
            void Callback(PlistHandle commandHandle, PlistHandle statusHandle, IntPtr userData)
            {
                var writer = channel!.Writer;
                if (InstallationProxyOperationException.TryFromStatusPlist(statusHandle, out var ex))
                {
                    writer.Complete(ex);
                }
                instproxy_status_get_current_list(statusHandle, out _, out _, out _, out var itemsHandle);
                var items = (PlistArray)PlistNode.From(itemsHandle)!.Clone();
                foreach (PlistDictionary item in items)
                {
                    var app = new Application(Device, item);
                    if (showHidden || app.IsVisible)
                    {
                        writer.TryWrite(app);
                    }
                }
                instproxy_status_get_name(statusHandle, out var name);
                if (name.Equals("Complete", StringComparison.InvariantCulture))
                {
                    writer.Complete();
                    callback = null;
                }
                items.Close();
            }
            using var dic = options?.ToDictionary();
            var hresult = instproxy_browse_with_callback(Handle, dic?.Handle ?? PlistHandle.Zero, callback, IntPtr.Zero);
            if (hresult.IsError())
                throw hresult.GetException();
            return channel.Reader.ReadAllAsync();
        }

        public IAsyncEnumerable<Application> GetApplicationsAsync(InstalltionProxyLookupOptions options)
        {
            return GetApplicationsAsync(options, false);
        }

        public IAsyncEnumerable<Application> GetApplicationsAsync(bool showHidden)
        {
            return GetApplicationsAsync(null, showHidden);
        }

        public IAsyncEnumerable<Application> GetApplicationsAsync()
        {
            return GetApplicationsAsync(null, false);
        }
#endif

        public CapabilityMatcher CheckCapabilityMatch(params string[] capabilities)
        {
            return CheckCapabilityMatch(capabilities);
        }

#if NET5_0_OR_GREATER
        public CapabilityMatcher CheckCapabilityMatch(IReadOnlySet<string> capabilities)
#else
        public CapabilityMatcher CheckCapabilityMatch(ISet<string> capabilities)
#endif
        {
            var nativecap = capabilities.Concat(new string?[] { null });
            var hresult = instproxy_check_capabilities_match(Handle, nativecap.ToArray(), PlistHandle.Zero, out var plistMatchHandle);
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

        public Task InstallAsync(string path)
        {
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            instproxy_install(Handle, path, PlistHandle.Zero, CallbackFactory.GetMethod(tcs), IntPtr.Zero);
            return tcs.Task;
        }

        public Task InstallAsync(string path, IProgress<int> progress)
        {
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            instproxy_install(Handle, path, PlistHandle.Zero, CallbackFactory.GetMethod(tcs, progress), IntPtr.Zero);
            return tcs.Task;
        }

        public Task UpgradeAsync(string path)
        {
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            instproxy_upgrade(Handle, path, PlistHandle.Zero, CallbackFactory.GetMethod(tcs), IntPtr.Zero);
            return tcs.Task;
        }

        public Task UpgradeAsync(string path, IProgress<int> progress)
        {
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            instproxy_upgrade(Handle, path, PlistHandle.Zero, CallbackFactory.GetMethod(tcs, progress), IntPtr.Zero);
            return tcs.Task;
        }

        public Task UninstallAsync(string bundleId)
        {
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            instproxy_uninstall(Handle, bundleId, PlistHandle.Zero, CallbackFactory.GetMethod(tcs), IntPtr.Zero);
            return tcs.Task;
        }

        public Task UninstallAsync(string bundleId, IProgress<int> progress)
        {
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            instproxy_uninstall(Handle, bundleId, PlistHandle.Zero, CallbackFactory.GetMethod(tcs, progress), IntPtr.Zero);
            return tcs.Task;
        }

        public Task ArchiveAsync(string bundleId, InstallationProxyArchiveOptions? options)
        {
            using var optDic = options?.ToDictionary();
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            instproxy_archive(Handle, bundleId, optDic?.Handle ?? PlistHandle.Zero, CallbackFactory.GetMethod(tcs), IntPtr.Zero);
            return tcs.Task;
        }

        public Task ArchiveAsync(string bundleId, InstallationProxyArchiveOptions? options, IProgress<int> progress)
        {
            using var optDic = options?.ToDictionary();
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            instproxy_archive(Handle, bundleId, optDic?.Handle ?? PlistHandle.Zero, CallbackFactory.GetMethod(tcs, progress), IntPtr.Zero);
            return tcs.Task;
        }

        public Task ArchiveAsync(string bundleId, IProgress<int> progress)
        {
            return ArchiveAsync(bundleId, null, progress);
        }

        public Task ArchiveAsync(string bundleId)
        {
            return ArchiveAsync(bundleId, options:null);
        }

        public Task RestoreAsync(string bundleId)
        {
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            instproxy_restore(Handle, bundleId, PlistHandle.Zero, CallbackFactory.GetMethod(tcs), IntPtr.Zero);
            return tcs.Task;
        }

        public Task RestoreAsync(string bundleId, IProgress<int> progress)
        {
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            instproxy_restore(Handle, bundleId, PlistHandle.Zero, CallbackFactory.GetMethod(tcs, progress), IntPtr.Zero);
            return tcs.Task;
        }
    }
}
