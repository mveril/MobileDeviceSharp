using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using SampleConsole.Commands.Handlers;

namespace SampleConsole.Commands
{
    internal class ProgramCommand : System.CommandLine.RootCommand
    {
        public Option<bool> WatchOption { get; }

        internal ProgramCommand() : base()
        {
            WatchOption = new Option<bool>("--watch", "Watch for device");
            AddGlobalOption(WatchOption);
            AddCommand(new ExploreCommand(WatchOption));
            AddCommand(new VersionCommand(WatchOption));
            AddCommand(new LanguageCommand(WatchOption));
            AddCommand(new LocaleCommand(WatchOption));
            AddCommand(new RebootCommand(WatchOption));
            AddCommand(new ListAppsCommand(WatchOption));
            AddCommand(new GetAppCommand(WatchOption));
            AddCommand(new OpenAppCommand(WatchOption));
            Handler = new UUIDCommandHandler(WatchOption);
        }
    }
}
