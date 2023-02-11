using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using System.Threading.Tasks;
using MobileDeviceSharp;

namespace SampleConsole.Commands.Handlers
{
    internal class UUIDCommandHandler : DeviceCommandHandlerBase
    {
        public UUIDCommandHandler(Option<bool> watchOption) : base(watchOption, false)
        {

        }
        protected override Task<int> OnDevice(IDevice device, InvocationContext context)
        {
            Console.WriteLine(device.Udid);
            return Task.FromResult(0);
        }
    }
}
