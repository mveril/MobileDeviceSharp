#if NETCOREAPP3_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using MobileDeviceSharp.PropertyList;

namespace MobileDeviceSharp.InstallationProxy.AsyncAdapter
{
    internal static class InstallationProxyOperationAsyncEnumerableAdapter
    {
        public static IAsyncEnumerable<PlistNode> BrowseAsync(InstallationProxySession session, InstalltionProxyLookupOptions? options, CancellationToken cancellationToken = default)
        {
            var channel = Channel.CreateUnbounded<PlistNode>(new UnboundedChannelOptions()
            {
                SingleReader = true,
                SingleWriter = true,

            });
            var progress = new ChannelWriterProgress(channel.Writer);
            var operation = new BrowseOperationTaskAdapter(session, progress, options);
            Task task = operation.AsTask();
            _ = task.ContinueWith(t =>
            {
                channel.Writer.Complete(t.Exception?.InnerException);
            }, TaskContinuationOptions.ExecuteSynchronously);
            return channel.Reader.ReadAllAsync(cancellationToken);
        }

        private class ChannelWriterProgress : IProgress<AsyncListProgress>
        {
            private readonly ChannelWriter<PlistNode> _writer;

            public ChannelWriterProgress(ChannelWriter<PlistNode> writer)
            {
                _writer = writer;
            }

            public void Report(AsyncListProgress value)
            {
                foreach (var item in value.list)
                {
                    _writer.TryWrite(item.Clone());
                }
            }
        }
    }
}
#endif
