using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using MobileDeviceSharp.InstallationProxy;

namespace SampleConsole.Commands.Handlers
{
    internal abstract class AppsCommandHanlderBase : DeviceCommandHandlerBase
    {
        protected AppsCommandHanlderBase(Option<bool> watchOption, bool needPair) : base(watchOption, needPair)
        {
        }

        public void PrintApp(Application application, IConsole console)
        {
            console.WriteLine($"{application.Name}: {application.BundleID} ({application.Version}) File sharing:{application.AllowFileShairing}");
        }
    }
}
