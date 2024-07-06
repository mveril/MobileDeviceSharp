using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MobileDeviceSharp.InstallationProxy.Native;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.InstallationProxy.Native.InstallationProxy;


namespace MobileDeviceSharp.InstallationProxy.AsyncAdapter
{
    internal class ArchiveOperationTaskAdapter : ApplicationProcessingOperationAdapter<InstallationProxyArchiveOptions>
    {
        public ArchiveOperationTaskAdapter(InstallationProxySession session, string appId, IProgress<int>? progress, InstallationProxyArchiveOptions? options) : base(session, appId, progress, options)
        {
        }

        protected override InstallationProxyError OnRun(InstallationProxyClientHandle clientHandle, PlistHandle options, IntPtr handle, InstallationProxyStatusCallBack callback)
        {
            return instproxy_archive(clientHandle, AppId, options, callback, handle);
        }
    }
}
