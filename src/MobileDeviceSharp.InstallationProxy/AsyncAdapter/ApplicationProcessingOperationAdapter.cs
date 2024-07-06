using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MobileDeviceSharp.InstallationProxy.AsyncAdapter;
using MobileDeviceSharp.InstallationProxy.Native;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.InstallationProxy.Native.InstallationProxy;

namespace MobileDeviceSharp.InstallationProxy.AsyncAdapter
{
    internal abstract class ApplicationProcessingOperationAdapter<TOptions> : CoreApplicationProcessingOperationAdapter<TOptions> where TOptions : InstallationProxyOperationOptions
    {
        public ApplicationProcessingOperationAdapter(InstallationProxySession session, string appId, IProgress<int>? progress, TOptions? options) : base(session, progress, options)
        {
            AppId = appId;
        }

        public string AppId { get; }

    }
}
