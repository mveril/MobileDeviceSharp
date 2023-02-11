using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using SampleConsole.Commands.Handlers;

namespace SampleConsole.Commands
{
    internal class LanguageCommand : DeviceCommand
    {
        public LanguageCommand(Option<bool> watchOption) : base("language", "Show language name", watchOption, new LanguageCommandHandler(watchOption))
        {

        }
    }
}
