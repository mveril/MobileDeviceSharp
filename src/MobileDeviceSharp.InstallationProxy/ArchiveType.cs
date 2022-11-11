using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.InstallationProxy
{
    [Flags]
    public enum ArchiveType
    {
        Application,
        Documents,
        All = Application | Documents
    }
}
