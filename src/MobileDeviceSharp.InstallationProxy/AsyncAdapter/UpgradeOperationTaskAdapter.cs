using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MobileDeviceSharp.InstallationProxy.Native;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.InstallationProxy.Native.InstallationProxy;


namespace MobileDeviceSharp.InstallationProxy.AsyncAdapter
{
    internal class UpgradeOperationTaskAdapter : PackageProcessingOperationAdapter<InstallationProxyOperationOptions>
    {
        public UpgradeOperationTaskAdapter(InstallationProxySession session, string packagePath, IProgress<int>? progress) : base(session, packagePath, progress, null)
        {
        }

        protected override InstallationProxyError OnRun(InstallationProxyClientHandle clientHandle, PlistHandle options, IntPtr handle, InstallationProxyStatusCallBack callback)
        {
            return instproxy_upgrade(clientHandle, PackagePath, options, callback, handle);
        }
    }
}
