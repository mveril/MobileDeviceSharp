using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using System.Threading.Tasks;
using MobileDeviceSharp;

namespace SampleConsole.Commands.Handlers
{
    internal class VersionCommandHandler : DeviceCommandHandlerBase
    {
        internal VersionCommandHandler(Option<bool> watchOption, Option<bool> buildNumberOption) : base(watchOption,true)
        {
            BuildNumberOption = buildNumberOption;
        }

        public Option<bool> BuildNumberOption { get; }

        protected override Task<int> OnDevice(IDevice device, InvocationContext context)
        {
            var showBuildNumber = context.ParseResult.GetValueForOption(BuildNumberOption);
            context.Console.WriteLine(device.OSVersion.ToString(showBuildNumber));
            return Task.FromResult(0);
        }
    }
}
