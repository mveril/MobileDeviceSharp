using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using MobileDeviceSharp.InstallationProxy;
using SampleConsole.Commands.Handlers;

namespace SampleConsole.Commands
{
    internal class ListAppsCommand : DeviceCommand
    {
        public ListAppsCommand(Option<bool> watchOption) : base("list-apps", "List the apps", watchOption, null)
        {
            AppType = new Option<string>("--apptype", () => ApplicationType.Any.ToString(), $"the app type in all tese values {string.Join(", ", Enum.GetNames(typeof(ApplicationType)))}");
            AddOption(AppType);
            Visible = new Option<bool>("--only-visible", () => true, $"Show visible apps only");
            AddOption(Visible);
            Handler = new ListAppsCommandHandler(watchOption,AppType,Visible);
        }

        public Option<string> AppType { get; }
        public Option<bool> Visible { get; }
    }
}
