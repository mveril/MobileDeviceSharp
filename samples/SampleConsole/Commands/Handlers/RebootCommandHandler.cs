using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Data.SqlTypes;
using System.Text;
using System.Threading.Tasks;
using MobileDeviceSharp;
using MobileDeviceSharp.DiagnosticsRelay;

namespace SampleConsole.Commands.Handlers
{
    internal class RebootCommandHandler : DeviceCommandHandlerBase
    {
        public RebootCommandHandler(Option<bool> watchOption, Option<bool> autoYes) : base(watchOption, true)
        {
            AutoYes = autoYes;
        }

        public Option<bool> AutoYes { get; }

        protected override Task<int> OnDevice(IDevice device, InvocationContext context)
        {
            var reboot = false;
            if (context.ParseResult.GetValueForOption(AutoYes))
            {
                reboot = true;
            }
            else
            {
                Console.WriteLine("Do you whant to reboot the device");
                if (Console.ReadLine().Trim(' ', '\n').Equals("y", StringComparison.InvariantCultureIgnoreCase))
                {
                    reboot = true;
                }
            }
            if (reboot)
            {
                device.Reboot();
            }
            return Task.FromResult(0);
        }
    }
}
