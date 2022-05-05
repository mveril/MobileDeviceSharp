using IOSLib.Native;
using static IOSLib.Native.NotificationProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using ConcurrentCollections;

namespace IOSLib
{
    public abstract partial class NotificationProxySessionBase
    {
#if NET5_0_OR_GREATER
        ConcurrentDictionary<string, ConcurrentBag<TaskCompletionSource>> _tasksDic = new();
#else
        ConcurrentDictionary<string, ConcurrentBag<TaskCompletionSource<object?>>> _tasksDic = new();
#endif

        /// <summary>
        /// Run these method to define the notification we want to observe a lot of constants are available on <see cref="NotificationProxyEvents.Recevable"/>
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="token"></param>
        public async Task ObserveNotificationAsync(string notification, CancellationToken token)
        {
#if NET5_0_OR_GREATER
            var tcs = new TaskCompletionSource();
#else
            var tcs = new TaskCompletionSource<object?>();
#endif
            _tasksDic.AddOrUpdate(notification, (key) => BagFactory(tcs), (_, value) => BagFactory(value, tcs));
            token.Register(() => tcs.TrySetCanceled(token));
            UpdateObservation();
            await tcs.Task.ConfigureAwait(false);
        }
#if NET5_0_OR_GREATER
        private static ConcurrentBag<TaskCompletionSource> BagFactory(TaskCompletionSource tcs)
#else
        private ConcurrentBag<TaskCompletionSource<object?>> BagFactory(TaskCompletionSource<object?> tcs)
#endif
        {
            return BagFactory(new(), tcs);
        }
#if NET5_0_OR_GREATER
        private static ConcurrentBag<TaskCompletionSource> BagFactory(ConcurrentBag<TaskCompletionSource> bag, TaskCompletionSource tcs)
#else
        private ConcurrentBag<TaskCompletionSource<object?>> BagFactory(ConcurrentBag<TaskCompletionSource<object?>> bag, TaskCompletionSource<object?> tcs)
#endif
        {
            bag.Add(tcs);
            return bag;
        }

        /// <summary>
        /// Run these method to define the notification we want to observe a lot of constants are available on <see cref="NotificationProxyEvents.Recevable"/>
        /// </summary>
        /// <param name="notification"></param>
        public async Task ObserveNotificationAsync(string notification)
        {
            await ObserveNotificationAsync(notification, CancellationToken.None);
        }

        private void TaskCallBack(string notification, IntPtr userData)
        {
            if(_tasksDic.TryGetValue(notification, out var tcss))
            {
                while (tcss.TryTake(out var tcs))
                {
#if NET5_0_OR_GREATER
                    tcs.TrySetResult();
#else
                    tcs.TrySetResult(null);
#endif
                }
            }
        }
    }
}
