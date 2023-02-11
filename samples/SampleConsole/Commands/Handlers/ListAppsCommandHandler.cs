using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using System.Threading.Tasks;
using MobileDeviceSharp;
using MobileDeviceSharp.InstallationProxy;

namespace SampleConsole.Commands.Handlers
{
    internal class ListAppsCommandHandler : AppsCommandHanlderBase
    {
        public ListAppsCommandHandler(Option<bool> watchOption, Option<string> appType, Option<bool> visible) : base(watchOption, true)
        {
            AppType= appType;
            Visible = visible;
        }

        public Option<string> AppType { get; }
        public Option<bool> Visible { get; }

        protected override async Task<int> OnDevice(IDevice device, InvocationContext context)
        {
            var appType = context.ParseResult.GetValueForOption(AppType);
            var visible = context.ParseResult.GetValueForOption(Visible);
            using var instproxysession = new InstallationProxySession(device);
#if NETCOREAPP3_1_OR_GREATER
                var apps = instproxysession.GetApplicationsAsync(new InstalltionProxyLookupOptions() { ApplicationType = (ApplicationType)Enum.Parse(typeof(ApplicationType), appType) }, !visible);
                await foreach (var app in apps)
#else
            var apps = instproxysession.GetApplications(new InstalltionProxyLookupOptions() { ApplicationType = (ApplicationType)Enum.Parse(typeof(ApplicationType), appType) }, !visible);
            foreach (var app in apps)
#endif
            {
                PrintApp(app, context.Console);
            }
            return 0;
        }
    }
}
