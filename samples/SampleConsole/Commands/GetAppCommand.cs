using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using SampleConsole.Commands.Handlers;

namespace SampleConsole.Commands
{
    internal class GetAppCommand : DeviceCommand
    {
        public GetAppCommand(Option<bool> watchOption) : base("get-apps", string.Empty, watchOption, null)
        {
            AppIds = new Argument<string[]>("appids", "The Application bundle identifier");
            AddArgument(AppIds);
            Handler = new GetAppCommandHandler(WatchOption, AppIds);
        }

        public Argument<string[]> AppIds { get; }
    }
}
