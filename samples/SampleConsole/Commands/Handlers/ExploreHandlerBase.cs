using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using MobileDeviceSharp.AFC;

namespace SampleConsole.Commands.Handlers
{
    internal abstract class ExploreHandlerBase : DeviceCommandHandlerBase
    {
        protected ExploreHandlerBase(Option<bool> watchOption) : base(watchOption, true)
        {

        }

        protected void StartExplore(AFCDirectory directory, IConsole console)
        {
            Explorer.Start(directory, console);
        }
    }
}
