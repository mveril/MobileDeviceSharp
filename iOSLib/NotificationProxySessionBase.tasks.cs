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
        ConcurrentDictionary<string, ConcurrentQueue<SemaphoreSlim>> _tasksDic = new();

        /// <summary>
        /// Run these method to define the notification we want to observe a lot of constants are available on <see cref="NotificationProxyEvents.Recevable"/>
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
                _tasksDic.AddOrUpdate(notification, (_) => QueueFactory(sem), (_, value) => QueueFactory(sem));
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
        /// Run these method to define the notification we want to observe a lot of constants are available on <see cref="NotificationProxyEvents.Recevable"/>
        /// </summary>
        /// <param name="notification"></param>
        public async Task ObserveNotificationAsync(string notification)
        {
            await ObserveNotificationAsync(notification, CancellationToken.None);
        }

        private void TaskCallBack(string notification)
        {
            if (_tasksDic.TryGetValue(notification, out var tcss))
            {
                while (tcss.TryDequeue(out var sem))
                {
                    sem.Release();
                }
            }
        }
    }
}
