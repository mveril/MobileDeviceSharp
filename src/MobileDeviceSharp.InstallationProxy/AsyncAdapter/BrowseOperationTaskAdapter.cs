using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MobileDeviceSharp.InstallationProxy.Native;
using MobileDeviceSharp.PropertyList;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.InstallationProxy.Native.InstallationProxy;

namespace MobileDeviceSharp.InstallationProxy.AsyncAdapter
{
    internal class BrowseOperationTaskAdapter : InstallationProxyOperationTaskAdapter<InstalltionProxyLookupOptions, AsyncListProgress>
    {
        public BrowseOperationTaskAdapter(InstallationProxySession session, IProgress<AsyncListProgress>? progress, InstalltionProxyLookupOptions options) : base(session, progress, options)
        {

        }

        protected override AsyncListProgress GetProgress(PlistHandle command, PlistHandle status)
        {
            instproxy_status_get_current_list(status, out var total, out var currentIndex, out var currentAmount, out var list);
            return new AsyncListProgress(total, currentIndex, currentAmount, new PlistArray(list));
        }
        protected override InstallationProxyError OnRun(InstallationProxyClientHandle clientHandle, PlistHandle options, IntPtr handle, InstallationProxyStatusCallBack callback)
        {
            return instproxy_browse_with_callback(clientHandle, options, callback, handle);
        }
    }
}
