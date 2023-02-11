using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using MobileDeviceSharp;
using MobileDeviceSharp.AFC;
using System.CommandLine;
using System.Threading;
using MobileDeviceSharp.DiagnosticsRelay;
using MobileDeviceSharp.InstallationProxy;
using System.Collections.Generic;
using MobileDeviceSharp.HouseArrest;
using SampleConsole.Commands;

namespace SampleConsole
{
    static class Program
    {
        static  async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var c = new ProgramCommand();
            await c.InvokeAsync(args);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject);
        }

        private static Task HandlerBase(bool watch, Func<IDevice, Task> task, bool needPair)
        {
            if (watch)
            {
                return Watch(task,needPair);
            }
            else
            {
                return Loop(task,needPair);
            }
        }

        private async static Task Loop(Func<IDevice, Task> task, bool needPair)
        {
            foreach (var device in IDevice.List())
            {
                Console.WriteLine(device.Name);
                if (needPair && !device.IsPaired)
                {
                    using (var ld = new LockdownSession(device))
                    {
                        await ld.PairAsync();
                    }
                }
                await task(device);
            }
        }

        private async static Task Watch(Func<IDevice, Task> task, bool needPair)
        {
            async void device_added(object sender, DeviceEventArgs e)
            {
                if (e.TryGetDevice(out var device))
                {
                    Console.WriteLine(device.Name);
                    if (needPair && !device.IsPaired)
                    {
                        using (var ld = new LockdownSession(device))
                        {
                            await ld.PairAsync();
                        }
                    }
                    await task(device);
                    device.Dispose();
                }
            }
            var watcher = new DeviceWatcher(MobileDeviceSharp.Usbmuxd.Native.IDeviceLookupOptions.All);
            watcher.DeviceAdded += device_added;
            watcher.Start();
            var semaphore = new SemaphoreSlim(0);
            Console.CancelKeyPress+=(sender,e)=>semaphore.Release();
            await semaphore.WaitAsync();
        }
    }
}
