using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using SampleConsole.Commands.Handlers;

namespace SampleConsole.Commands
{
    internal class VersionCommand : DeviceCommand
    {
        internal VersionCommand(Option<bool> watchOption) : base("version", "Show the OS version", watchOption, null)
        {
            BuildNumberOption = new Option<bool>("--build", "Show build number");
            AddOption(BuildNumberOption);
            Handler = new VersionCommandHandler(watchOption, BuildNumberOption);
        }
        public Option<bool> BuildNumberOption { get; }
}
}
