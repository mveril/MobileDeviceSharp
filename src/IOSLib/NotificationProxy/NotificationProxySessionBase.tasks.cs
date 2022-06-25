using IOSLib.Native;
using static IOSLib.NotificationProxy.Native.NotificationProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using ConcurrentCollections;
using IOSLib.NotificationProxy.Native;

namespace IOSLib.NotificationProxy
{
    public abstract partial class NotificationProxySessionBase
    {
        ConcurrentDictionary<string, ConcurrentQueue<SemaphoreSlim>> _tasksDic = new();

        /// <summary>
        /// Define the notification we want to observe asyncroniously. A lot of constants are available on <see cref="NotificationProxyEvents.Recevable"/>
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="token"></param>
        public async Task ObserveNotificationAsync(string notification, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var result = np_observe_notification(Handle, notification);
            if (result.IsError())
            {
                throw result.GetException();
            }
            else
            {
                var sem = new SemaphoreSlim(0);
                _tasksDic.AddOrUpdate(notification, (_) => QueueFactory(sem), (_, value) => QueueFactory(value, sem));
                await sem.WaitAsync(token);
            }
        }
        private static ConcurrentQueue<SemaphoreSlim> QueueFactory(SemaphoreSlim sem)
        {
            return QueueFactory(new(), sem);
        }
        private static ConcurrentQueue<SemaphoreSlim> QueueFactory(ConcurrentQueue<SemaphoreSlim> queue, SemaphoreSlim sem)
        {
            queue.Enqueue(sem);
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
            if (_tasksDic.TryGetValue(notification, out var sems))
            {
                while (sems.TryDequeue(out var sem))
                {
                    sem.Release();
                }
            }
        }
    }
}
