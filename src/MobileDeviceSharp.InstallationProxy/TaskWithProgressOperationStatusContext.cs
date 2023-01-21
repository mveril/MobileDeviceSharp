using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.InstallationProxy.Native.InstallationProxy;

namespace MobileDeviceSharp.InstallationProxy
{
    internal class TaskWithProgressOperationStatusContext : OperationStatusContext
    {
#if NET5_0_OR_GREATER
        public TaskCompletionSource Tcs { get; }
#else
        public TaskCompletionSource<object?> Tcs { get; }
#endif
        public IProgress<int>? Progress { get; }
#if NET5_0_OR_GREATER
        public TaskWithProgressOperationStatusContext(TaskCompletionSource taskCompletionSource, IProgress<int>? progress)
#else
        public TaskWithProgressOperationStatusContext(TaskCompletionSource<object?> taskCompletionSource, IProgress<int>? progress)
#endif
        {
            Tcs = taskCompletionSource;
            Progress = progress;
        }

#if NET5_0_OR_GREATER
        public TaskWithProgressOperationStatusContext(TaskCompletionSource taskCompletionSource)
#else
        public TaskWithProgressOperationStatusContext(TaskCompletionSource<object?> taskCompletionSource)
#endif
            : this(taskCompletionSource, null)
        {
        }

        protected override void OnException(InstallationProxyOperationException exception) => Tcs.SetException(exception);
        protected override void OnUpdateProgress(PlistHandle command, PlistHandle status)
        {
            if (Progress is not null)
            {
                instproxy_status_get_percent_complete(status, out int percent);
                Progress.Report(percent);
            }
            
        }
        protected override void OnCompleted(PlistHandle command, PlistHandle status)
#if NET5_0_OR_GREATER
            => Tcs.SetResult();
#else
            => Tcs.SetResult(null);
#endif
    }
}
