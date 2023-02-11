using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using SampleConsole.Commands.Handlers;

namespace SampleConsole.Commands
{
    internal class RebootCommand : DeviceCommand
    {
        public RebootCommand(Option<bool> watchOption) : base("reboot", "Reboot the device with a confirmation", watchOption, null)
        {
            AutoYes = new Option<bool>("-y", "Dont ask for confirmation");
            Handler = new RebootCommandHandler(watchOption, AutoYes);
        }
        public Option<bool> AutoYes { get; }
    }

}
