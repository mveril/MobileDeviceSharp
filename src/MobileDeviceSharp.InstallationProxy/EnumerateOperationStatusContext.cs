using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using MobileDeviceSharp.PropertyList;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.InstallationProxy.Native.InstallationProxy;

namespace MobileDeviceSharp.InstallationProxy
{
    internal class EnumerateOperationStatusContext : OperationStatusContext
    {
        public EnumerateOperationStatusContext(ChannelWriter<PlistNode> writer)
        {
            Writer = writer;
        }

        public ChannelWriter<PlistNode> Writer { get; }

        protected override void OnCompleted(PlistHandle command, PlistHandle status) => Writer.Complete();
        protected override void OnException(InstallationProxyOperationException exception) => Writer.Complete(exception);
        protected override void OnUpdateProgress(PlistHandle command, PlistHandle status)
        {
            instproxy_status_get_current_list(status, out _, out _, out _, out var arrayHandle);
            if (!arrayHandle.IsInvalid)
            {
                    using var array = (PlistArray)PlistNode.From(arrayHandle)!;
                    foreach (var item in array)
                    {
                        Writer.TryWrite(item.Clone());
                    }
            }
        }
    }
}
