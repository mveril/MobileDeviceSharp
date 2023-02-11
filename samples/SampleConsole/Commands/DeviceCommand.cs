using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using SampleConsole.Commands.Handlers;

namespace SampleConsole.Commands
{
    internal abstract class DeviceCommand : Command
    {
        protected DeviceCommand(string name, string description,Option<bool> watchOption, DeviceCommandHandlerBase? handler) : base(name, description)
        {
            WatchOption = watchOption;
            Handler = handler;
        }
        public Option<bool> WatchOption { get; }
    }
}
