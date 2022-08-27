using MobileDeviceSharp.Native;
using static MobileDeviceSharp.NotificationProxy.Native.NotificationProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using ConcurrentCollections;
using MobileDeviceSharp.NotificationProxy.Native;

namespace MobileDeviceSharp.NotificationProxy
{
    public abstract partial class NotificationProxySessionBase
    {
#if NET5_0_OR_GREATER
        ConcurrentDictionary<string, ConcurrentQueue<TaskCompletionSource>> _tasksDic = new();
#else
        ConcurrentDictionary<string, ConcurrentQueue<TaskCompletionSource<object?>>> _tasksDic = new();
#endif

        /// <summary>
        /// Define the notification we want to observe asyncroniously. A lot of constants are available on <see cref="NotificationProxyEvents.Recevable"/>
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
                var tsk = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
#else
                var tsk = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
#endif
                token.Register(() => tsk.TrySetCanceled(token));
                _tasksDic.AddOrUpdate(notification, (_) => QueueFactory(tsk), (_, value) => QueueFactory(value, tsk));
                return tsk.Task;
            }
        }
#if NET5_0_OR_GREATER
        private static ConcurrentQueue<TaskCompletionSource> QueueFactory(TaskCompletionSource tsk)
#else
        private static ConcurrentQueue<TaskCompletionSource<object?>> QueueFactory(TaskCompletionSource<object?> tsk)
#endif
        {
            return QueueFactory(new(), tsk);
        }
#if NET5_0_OR_GREATER
        private static ConcurrentQueue<TaskCompletionSource> QueueFactory(ConcurrentQueue<TaskCompletionSource> queue, TaskCompletionSource tsk)
#else
        private static ConcurrentQueue<TaskCompletionSource<object?>> QueueFactory(ConcurrentQueue<TaskCompletionSource<object?>> queue, TaskCompletionSource<object?> tsk)
#endif
        {
            queue.Enqueue(tsk);
            return queue;
        }

        /// <summary>
        /// Define the notification we want to observe asyncroniously. A lot of constants are available on <see cref="NotificationProxyEvents.Recevable"/>
        /// </summary>
        /// <param name="notification"></param>
        public async Task ObserveNotificationAsync(string notification)
        {
            await ObserveNotificationAsync(notification, CancellationToken.None);
        }

        private void TaskCallBack(string notification)
        {
            if (_tasksDic.TryGetValue(notification, out var tsks))
            {
                while (tsks.TryDequeue(out var tsk))
                {
#if NET5_0_OR_GREATER
                    tsk.TrySetResult();
#else
                    tsk.TrySetResult(null);
#endif
                }
            }
        }
    }
}
