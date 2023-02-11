using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using MobileDeviceSharp.AFC;
using SampleConsole.Commands.Handlers;

namespace SampleConsole.Commands
{
    internal class ExploreCommand : DeviceCommand
    {
        public ExploreCommand(Option<bool> watchOption) : base("explore", "Explore device files",watchOption, new ExploreCommandHandler(watchOption))
        {

        }
    }
}
