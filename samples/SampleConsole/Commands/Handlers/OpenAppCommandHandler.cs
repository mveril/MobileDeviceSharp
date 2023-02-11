using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using System.Threading.Tasks;
using MobileDeviceSharp;
using MobileDeviceSharp.HouseArrest;
using MobileDeviceSharp.InstallationProxy;

namespace SampleConsole.Commands.Handlers
{
    internal class OpenAppCommandHandler : ExploreCommandHandler
    {
        public OpenAppCommandHandler(Option<bool> watchOption, Argument<string> appId) : base(watchOption)
        {
            AppId = appId;
        }

        public Argument<string> AppId { get; }

        protected override Task<int> OnDevice(IDevice device, InvocationContext context)
        {
            using var instproxysession = new InstallationProxySession(device);
            var myApp = instproxysession.GetApplication(context.ParseResult.GetValueForArgument(AppId));
            using var houseArrest = new HouseArrestSession(myApp, HouseArrestLocation.Documents);
            using var afcSession = houseArrest.AFCSession;
            StartExplore(afcSession.Documents,context.Console);
            return Task.FromResult(0);
        }
    }
}
