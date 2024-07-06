using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MobileDeviceSharp.InstallationProxy.AsyncAdapter;
using MobileDeviceSharp.InstallationProxy.Native;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.InstallationProxy.Native.InstallationProxy;

namespace MobileDeviceSharp.InstallationProxy.AsyncAdapter
{
    internal abstract class InstallationProxyOperationTaskAdapter<TOptions, TProgress> where TOptions : InstallationProxyOperationOptions
    {
#if NET5_0_OR_GREATER
        private TaskCompletionSource _tcs;
#else
        private TaskCompletionSource<object?> _tcs;
#endif

        protected InstallationProxyOperationTaskAdapter(InstallationProxySession session, IProgress<TProgress>? progress, TOptions? options)
        {
            Session = session;
            _progress = progress;
            _handle = GCHandle.Alloc(this, GCHandleType.Normal);
            Options = options;
            _optionHandle = Options is null ? PlistHandle.Zero : Options.ToDictionary().Handle;
#if NET5_0_OR_GREATER
            _tcs = new TaskCompletionSource();
#else
            _tcs = new TaskCompletionSource<object?>();
#endif
            var hresult = OnRun(Session.Handle, _optionHandle, (IntPtr)_handle, _nativeCallback);
            if (hresult.IsError())
            {
                _handle.Free();
                _tcs.SetException(hresult.GetException());
            }
        }

        private static void NativeCallback(PlistHandle command, PlistHandle status, IntPtr userData)
        {
            if (userData != IntPtr.Zero)
            {
                var handle = GCHandle.FromIntPtr(userData);
                if (handle.Target is InstallationProxyOperationTaskAdapter<TOptions, TProgress> _self)
                {
                    _self.InstanceCallback(command, status);
                }
            }
        }
        protected abstract InstallationProxyError OnRun(InstallationProxyClientHandle clientHandle, PlistHandle options, IntPtr handle, InstallationProxyStatusCallBack callback);

        public InstallationProxySession Session { get; }
        private IProgress<TProgress>? _progress { get; }
        public TOptions? Options { get; }

        private readonly PlistHandle _optionHandle;
        private readonly GCHandle _handle;
        private readonly InstallationProxyClientHandle _clientHandle;
        private readonly TOptions _options;
        private static readonly InstallationProxyStatusCallBack _nativeCallback = NativeCallback;

        internal void InstanceCallback(PlistHandle command, PlistHandle status)
        {
            var ex = GetException(status);
            if (ex is not null)
            {
                _tcs.SetException(ex);
                _handle.Free();
            }
            if (_progress is not null)
            {
                TProgress progressValue = GetProgress(command, status);
                _progress.Report(progressValue);
            }
            if (IsComplete(status))
            {
#if NET5_0_OR_GREATER
                _tcs.SetResult();
#else
                _tcs.SetResult(null);
#endif
                _handle.Free();
            }
        }

        protected abstract TProgress GetProgress(PlistHandle command, PlistHandle status);

        private static InstallationProxyOperationException? GetException(PlistHandle status)
        {
            instproxy_status_get_error(status, out var errorName, out var errorDescription, out var errorCode);
            return errorName is null ? null : new InstallationProxyOperationException(errorName, errorDescription, errorCode);
        }

        internal static bool IsComplete(PlistHandle status)
        {
            instproxy_status_get_name(status, out string statusName);
            return statusName is not null && statusName.Equals("Complete", StringComparison.InvariantCultureIgnoreCase);
        }

        public Task AsTask()
        {
            return _tcs.Task;
        }

        public static explicit operator Task(InstallationProxyOperationTaskAdapter<TOptions, TProgress> v) => v.AsTask();

        public TaskAwaiter GetAwaiter()
        {
            return AsTask().GetAwaiter();
        }
    }
}
