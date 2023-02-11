using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using System.Threading.Tasks;
using MobileDeviceSharp;
using MobileDeviceSharp.AFC;

namespace SampleConsole.Commands.Handlers
{
    internal class ExploreCommandHandler : ExploreHandlerBase
    {
        public ExploreCommandHandler(Option<bool> watchOption) : base(watchOption)
        {

        }

        protected override Task<int> OnDevice(IDevice device, InvocationContext context)
        {
            var afc = new AFCSession(device);
            StartExplore(afc.Root, context.Console);
            return Task.FromResult(0);
        }
    }
}
