using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MobileDeviceSharp;

namespace SampleConsole.Commands.Handlers
{
    internal abstract class DeviceCommandHandlerBase : ICommandHandler
    {
        public Option<bool> WatchOption { get; }

        public bool NeedPair { get; }

        public DeviceCommandHandlerBase(Option<bool> watchOption, bool needPair)
        {
            WatchOption = watchOption;
            NeedPair = needPair;
        }
        public Task<int> InvokeAsync(InvocationContext context)
        {
            if (context.ParseResult.GetValueForOption(WatchOption))
            {
                return Watch(context);
            }
            else
            {
                return Loop(context);
            }
        }


        private async Task<int> Loop(InvocationContext context)
        {
            foreach (var device in IDevice.List())
            {
                Console.WriteLine(device.Name);
                if (NeedPair && !device.IsPaired)
                {
                    using (var ld = new LockdownSession(device))
                    {
                        await ld.PairAsync();
                    }
                }
                var result = await OnDevice(device, context);
                if (result != 0)
                {
                    return result;
                }
            }
            return 0;
        }

        protected async Task<int> Watch(InvocationContext context)
        {
            async void device_added(object sender, DeviceEventArgs e)
            {
                if (e.TryGetDevice(out var device))
                {
                    Console.WriteLine(device.Name);
                    if (NeedPair && !device.IsPaired)
                    {
                        using (var ld = new LockdownSession(device))
                        {
                            await ld.PairAsync();
                        }
                    }
                    _ = await OnDevice(device, context);
                    device.Dispose();
                }
            }
            var watcher = new DeviceWatcher(MobileDeviceSharp.Usbmuxd.Native.IDeviceLookupOptions.All);
            watcher.DeviceAdded += device_added;
            watcher.Start();
            var semaphore = new SemaphoreSlim(0);
            Console.CancelKeyPress += (sender, e) => semaphore.Release();
            await semaphore.WaitAsync();
            return 0;
        }

        protected abstract Task<int> OnDevice(IDevice device, InvocationContext context);
    }
}
