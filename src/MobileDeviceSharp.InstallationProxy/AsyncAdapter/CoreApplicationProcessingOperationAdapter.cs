using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MobileDeviceSharp.InstallationProxy.Native;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.InstallationProxy.Native.InstallationProxy;

namespace MobileDeviceSharp.InstallationProxy.AsyncAdapter
{
    internal abstract class CoreApplicationProcessingOperationAdapter<TOptions> : InstallationProxyOperationTaskAdapter<TOptions, int> where TOptions : InstallationProxyOperationOptions
    {
        protected CoreApplicationProcessingOperationAdapter(InstallationProxySession session, IProgress<int>? progress, TOptions? options) : base(session, progress, options)
        {
        }

        protected override int GetProgress(PlistHandle command, PlistHandle status)
        {
            instproxy_status_get_percent_complete(status, out var percent);
            return percent;
        }
    }
}
