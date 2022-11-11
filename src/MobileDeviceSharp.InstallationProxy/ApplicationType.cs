using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.InstallationProxy
{
    [Flags]
    public enum ApplicationType
    {
        Hidden =1,
        System,
        User,
        Any = Hidden | User | System,
    }
}
