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
        ConcurrentDictionary<string, ConcurrentQueue<TaskCompletionSource>> _tasksDic = new();
#else
        ConcurrentDictionary<string, ConcurrentQueue<TaskCompletionSource<object?>>> _tasksDic = new();
#endif

        /// <summary>
        /// Run these method to define the notification we want to observe a lot of constants are available on <see cref="NotificationProxyEvents.Recevable"/>
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="token"></param>
        public Task ObserveNotificationAsync(string notification, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return Task.FromCanceled(token);
            }
            var result = np_observe_notification(Handle, notification);
            if (result.IsError())
            {
                return Task.FromException(result.GetException());
            }
            else
            {
#if NET5_0_OR_GREATER
                var tcs = new TaskCompletionSource();
#else
                var tcs = new TaskCompletionSource<object?>();
#endif
                token.Register(() => tcs.TrySetCanceled(token));
                _tasksDic.AddOrUpdate(notification, (_) => QueueFactory(tcs), (_, value) => QueueFactory(value, tcs));
                return tcs.Task;
            }
        }
#if NET5_0_OR_GREATER
        private static ConcurrentQueue<TaskCompletionSource> QueueFactory(TaskCompletionSource tcs)
#else
        private ConcurrentQueue<TaskCompletionSource<object?>> QueueFactory(TaskCompletionSource<object?> tcs)
#endif
        {
            return QueueFactory(new(), tcs);
        }
#if NET5_0_OR_GREATER
        private static ConcurrentQueue<TaskCompletionSource> QueueFactory(ConcurrentQueue<TaskCompletionSource> queue, TaskCompletionSource tcs)
#else
        private ConcurrentQueue<TaskCompletionSource<object?>> QueueFactory(ConcurrentQueue<TaskCompletionSource<object?>> queue, TaskCompletionSource<object?> tcs)
#endif
        {
            queue.Enqueue(tcs);
            return queue;
        }

        /// <summary>
        /// Run these method to define the notification we want to observe a lot of constants are available on <see cref="NotificationProxyEvents.Recevable"/>
        /// </summary>
        /// <param name="notification"></param>
        public async Task ObserveNotificationAsync(string notification)
        {
            await ObserveNotificationAsync(notification, CancellationToken.None);
        }

        private void TaskCallBack(string notification)
        {
            if(_tasksDic.TryGetValue(notification, out var tcss))
            {
                while (tcss.TryDequeue(out var tcs))
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
