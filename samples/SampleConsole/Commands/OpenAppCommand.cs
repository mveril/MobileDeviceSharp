using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using SampleConsole.Commands.Handlers;

namespace SampleConsole.Commands
{
    internal class OpenAppCommand : DeviceCommand
    {
        public OpenAppCommand(Option<bool> watchOption) : base("open-app",string.Empty, watchOption, null)
        {
            AppId = new Argument<string>("appid", "The Application bundle identifier");
            AddArgument(AppId);
            Handler = new OpenAppCommandHandler(WatchOption, AppId);
        }

        public Argument<string> AppId { get; }
    }
}
