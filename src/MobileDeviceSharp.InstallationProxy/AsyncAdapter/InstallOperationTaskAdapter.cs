using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MobileDeviceSharp.InstallationProxy.Native;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.InstallationProxy.Native.InstallationProxy;


namespace MobileDeviceSharp.InstallationProxy.AsyncAdapter
{
    internal class InstallOperationTaskAdapter : PackageProcessingOperationAdapter<InstallationProxyInstallOptions>
    {
        public InstallOperationTaskAdapter(InstallationProxySession session, string packagePath, IProgress<int>? progress, InstallationProxyInstallOptions options) : base(session, packagePath, progress, options)
        {
        }

        protected override InstallationProxyError OnRun(InstallationProxyClientHandle clientHandle, PlistHandle options, IntPtr handle, InstallationProxyStatusCallBack callback)
        {
            return instproxy_install(clientHandle, PackagePath, options, callback, handle);
        }
    }
}
