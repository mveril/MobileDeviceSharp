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
    internal class GetAppCommandHandler : AppsCommandHanlderBase
    {
        public GetAppCommandHandler(Option<bool> watchOption, Argument<string[]> appIds) : base(watchOption, true)
        {
            AppIds = appIds;
        }

        public Argument<string[]> AppIds { get; }

        protected override Task<int> OnDevice(IDevice device, InvocationContext context)
        {
            using var instproxysession = new InstallationProxySession(device);
            var apps = instproxysession.GetApplications(context.ParseResult.GetValueForArgument(AppIds));
            foreach (var app in apps.Values)
            {
                PrintApp(app, context.Console);
            }
            return Task.FromResult(0);
        }
    }
}
