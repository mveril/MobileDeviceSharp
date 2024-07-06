using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MobileDeviceSharp.InstallationProxy.Native;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.InstallationProxy.Native.InstallationProxy;


namespace MobileDeviceSharp.InstallationProxy.AsyncAdapter
{
    internal class UninstallOperationTaskAdapter : ApplicationProcessingOperationAdapter<InstallationProxyOperationOptions>
    {
        public UninstallOperationTaskAdapter(InstallationProxySession session, string appId, IProgress<int>? progress) : base(session, appId, progress, null)
        {
        }

        protected override InstallationProxyError OnRun(InstallationProxyClientHandle clientHandle, PlistHandle options, IntPtr handle, InstallationProxyStatusCallBack callback)
        {
            return instproxy_uninstall(clientHandle, AppId, options, callback, handle);
        }
    }
}
