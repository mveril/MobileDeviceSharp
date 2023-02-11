using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using SampleConsole.Commands.Handlers;

namespace SampleConsole.Commands
{
    internal class LocaleCommand : DeviceCommand
    {
        public LocaleCommand(Option<bool> watchOption) :base("locale", "Show locale name", watchOption, new LocaleCommandHandler(watchOption))
        {

        }
    }
}
