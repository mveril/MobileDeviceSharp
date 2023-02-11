using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using System.Threading.Tasks;
using MobileDeviceSharp;

namespace SampleConsole.Commands.Handlers
{
    internal class LanguageCommandHandler : DeviceCommandHandlerBase
    {
        public LanguageCommandHandler(Option<bool> watchOption) : base(watchOption, true)
        {

        }

        protected override Task<int> OnDevice(IDevice device, InvocationContext context)
        {
            Console.WriteLine(device.Language);
            return Task.FromResult(0);
        }
    }
}
