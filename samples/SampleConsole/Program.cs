﻿using System;
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

namespace SampleConsole
{
    static class Program
    {
        static  async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var c = new RootCommand();
            var watchOption = new Option<bool>("--watch", "Watch for device");
            c.AddGlobalOption(watchOption);
            c.SetHandler((bool watch) => HandlerBase(watch, (device) =>
            {
                Console.WriteLine(device.Udid);
                return Task.CompletedTask;
            }, false), watchOption);
            var explore = new Command("explore", "Explore device files");
            explore.SetHandler((bool watch) => HandlerBase(watch, Explore, true), watchOption);
            c.Add(explore);
            var version = new Command("version","Show the OS version");
            var buildNumberOption = new Option<bool>("--build", "Show build number");
            version.AddOption(buildNumberOption);
            version.SetHandler((bool watch, bool showBuildNumber) => HandlerBase(watch, (device) =>
            {
                Console.WriteLine(device.OSVersion.ToString(showBuildNumber));
                return Task.CompletedTask;
            }, false), watchOption, buildNumberOption);
            c.Add(version);
            var language = new Command("language", "Show language name");
            language.SetHandler((bool watch) => HandlerBase(watch,(device) =>
            {
                Console.WriteLine(device.Language);
                return Task.CompletedTask;
            }, true), watchOption);
            c.Add(language);
            var locale = new Command("locale", "Show locale name");
            locale.SetHandler((bool watch) => HandlerBase(watch, (device) =>
            {
                Console.WriteLine(device.Locale);
                return Task.CompletedTask;
            }, true), watchOption);
            c.Add(locale);
            var reboot = new Command("reboot", "Reboot the device with a confirmation");
            var autoYesOption = new Option<bool>("-y", "Dont ask for confirmation");
            c.Add(reboot);
            reboot.AddOption(autoYesOption);
            reboot.SetHandler((bool watch, bool autoYes) => HandlerBase(watch, (device) =>
            {
                var reboot = false;
                if (autoYes)
                {
                    reboot = true;
                }
                else
                {
                    Console.WriteLine("Do you whant to reboot the device");
                    if (Console.ReadLine().Trim(' ','\n').Equals("y",StringComparison.InvariantCultureIgnoreCase))
                    {
                        reboot=true;
                    }
                }
                if (reboot)
                {
                    device.Reboot();
                }
                return Task.CompletedTask;
            }, true), watchOption, autoYesOption);
            var listapps = new Command("list-apps", "List the apps");
            var appType = new Option<string>("--apptype",()=>ApplicationType.Any.ToString(), $"the app type in all tese values {string.Join(", ",Enum.GetNames(typeof(ApplicationType)))}");
            var visible = new Option<bool>("--only-visible",()=> true, $"Show visible apps only");
            listapps.AddOption(appType);
            listapps.AddOption(visible);
            listapps.SetHandler((bool watch, string appType, bool visible) => HandlerBase(watch, async (device) =>
            {
                using var instproxysession = new InstallationProxySession(device);
#if NETCOREAPP3_1_OR_GREATER
                var apps = instproxysession.GetApplicationsAsync(new InstalltionProxyLookupOptions() { ApplicationType = (ApplicationType)Enum.Parse(typeof(ApplicationType), appType) }, !visible);
                await foreach (var app in apps)
#else
                var apps = instproxysession.GetApplications(new InstalltionProxyLookupOptions() { ApplicationType = (ApplicationType)Enum.Parse(typeof(ApplicationType), appType) }, !visible);
                foreach (var app in apps)
#endif
                {
                    Console.WriteLine($"{app.Name}: {app.BundleID} ({app.Version}) File sharing:{app.AllowFileShairing}");
                }
            }, false), watchOption, appType,visible);
            c.Add(listapps);
            var app = new Command("get-apps");
            var appIds = new Argument<string[]>("appids", "The Application bundle identifier");
            app.AddArgument(appIds);
            app.SetHandler((bool watch, string[] appIds) => HandlerBase(watch, async (device) =>
            {
                using var instproxysession = new InstallationProxySession(device);
                var apps = instproxysession.GetApplications(appIds);
                foreach (var app in apps.Values)
                {
                     Console.WriteLine($"{app.Name}: {app.BundleID} ({app.Version}) File sharing:{app.AllowFileShairing}");
                }
            }, false), watchOption, appIds);
            c.AddCommand(app);
            var openApp = new Command("open-app");
            var appId = new Argument<string>("appid", "The Application bundle identifier");
            openApp.AddArgument(appId);
            openApp.SetHandler((bool watch, string appId) => HandlerBase(watch, async (device) =>
            {
                using var instproxysession = new InstallationProxySession(device);
                var myApp = instproxysession.GetApplication(appId);
                using var houseArrest = new HouseArrestSession(myApp, HouseArrestLocation.Documents);
                using var afcSession = houseArrest.AFCSession;
                ProcessItem(afcSession.Documents);
            }, false), watchOption, appId);
            c.AddCommand(openApp);
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

        private static Task Explore(IDevice device)
        {
            var afc = new AFCSession(device);
            ProcessItem(afc.Root);
            return Task.CompletedTask;
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

        private static void ProcessLine(AFCDirectory current, string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                if (current.Parent is not null)
                {
                    ProcessItem(current.Parent);
                }
                else
                {
                    ProcessItem(current);
                }
            }
            else
            {
                ProcessItem(current.GetItem(line));
            }
        }

        private static void ShowFileInExplorer(string filePath)
        {
            try
            {
                var winDir = Environment.GetEnvironmentVariable("windir");
                if (winDir is not null)
                {
                    var explorerPath = Path.Combine(winDir, @"explorer.exe");
                    var arguments = string.Format("/select, {0}{1}{0}", (char)34, filePath);
                    Process.Start(explorerPath, arguments);
                }
            }
            catch (Exception )
            {
                //handle the exception your way!
            }
        }

        private static void ProcessItem(AFCDirectory Dir)
        {
            foreach (var item in Dir.GetItems())
            {
                Console.WriteLine(item.Path);
                Console.WriteLine($"> {item.CreationTime}");
                Console.WriteLine($"> {item.LastModifiedTime}");
                Console.WriteLine($"> {item.Name}");
                Console.WriteLine($"> {item.Extension}");
            }
            ProcessLine(Dir,Console.ReadLine());
        }

        private static void ProcessItem(AFCFile file)
        {
            var p = Path.Combine(Path.GetTempPath(), file.Name);
            var outp = File.OpenWrite(p);
            var inp = file.OpenRead();
            inp.CopyTo(outp);
            inp.Close();
            outp.Close();
            ShowFileInExplorer(p);
            ProcessItem(file.Parent);
        }
        private static void ProcessItem(AFCItem item)
        {
            if (item.GetType() == typeof(AFCFile))
            {
                ProcessItem((AFCFile)item);
            }
            else if (item.GetType() == typeof(AFCDirectory))
            {
                ProcessItem((AFCDirectory)item);
            }
        }
    }
}
