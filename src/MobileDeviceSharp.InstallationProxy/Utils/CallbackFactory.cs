using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MobileDeviceSharp.InstallationProxy;
using MobileDeviceSharp.InstallationProxy.Native;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.InstallationProxy.Native.InstallationProxy;

namespace MobileDeviceSharp.InstallationProxy
{
    internal static class CallbackFactory
    {
#if NET5_0_OR_GREATER
        internal static System.Collections.Concurrent.ConcurrentDictionary<TaskCompletionSource, InstallationProxyStatusCallBack> s_callbackDictionary = new();
#else
        internal static System.Collections.Concurrent.ConcurrentDictionary<TaskCompletionSource<object?>, InstallationProxyStatusCallBack> s_callbackDictionary = new();
#endif

#if NET5_0_OR_GREATER
        internal static InstallationProxyStatusCallBack GetMethod(TaskCompletionSource tcs, IProgress<int> progress)
#else
        internal static InstallationProxyStatusCallBack GetMethod(TaskCompletionSource<object?> tcs, IProgress<int> progress)
#endif
        {
            void Callback(PlistHandle command, PlistHandle status, IntPtr userData)
            {
                ReportException(tcs, status);
                ReportProgress(progress, status);
                ReportSuccess(tcs, status);
            };
            InstallationProxyStatusCallBack cb = Callback;
            s_callbackDictionary.TryAdd(tcs, cb);
            return cb;
        }
#if NET5_0_OR_GREATER
        internal static InstallationProxyStatusCallBack GetMethod(TaskCompletionSource tcs)
#else
        internal static InstallationProxyStatusCallBack GetMethod(TaskCompletionSource<object?> tcs)
#endif
        {
            void Callback(PlistHandle command, PlistHandle status, IntPtr userData)
            {
                ReportException(tcs, status);
                ReportSuccess(tcs, status);
            };
            return new InstallationProxyStatusCallBack(Callback);
        }

#if NET5_0_OR_GREATER
        private static void ReportException(TaskCompletionSource tcs,PlistHandle statusPlist)
#else
        private static void ReportException(TaskCompletionSource<object?> tcs, PlistHandle statusPlist)
#endif

        {
            if (InstallationProxyOperationException.TryFromStatusPlist(statusPlist, out var ex))
            {
                tcs.SetException(ex!);
                s_callbackDictionary.TryRemove(tcs, out _);
            };
        }
        private static void ReportProgress(IProgress<int> progress, PlistHandle statusPlist)
        {
            instproxy_status_get_percent_complete(statusPlist, out var percent);
            progress.Report(percent);
        }

#if NET5_0_OR_GREATER
        static void ReportSuccess(TaskCompletionSource tcs, PlistHandle statusPlist)
#else
        static void ReportSuccess(TaskCompletionSource<object?> tcs, PlistHandle statusPlist)
#endif
        {
            instproxy_status_get_name(statusPlist, out var name);
            if (name.Equals("Complete", StringComparison.InvariantCulture))
            {
#if NET5_0_OR_GREATER
                tcs.SetResult();
#else
                tcs.SetResult(default);
#endif
                s_callbackDictionary.TryRemove(tcs, out _);
            }
        }

    }
}
