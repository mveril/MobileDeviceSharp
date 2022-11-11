using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.PropertyList;

namespace MobileDeviceSharp.InstallationProxy
{
    public abstract class InstallationProxyOperationOptions
    {
        public abstract PlistDictionary? ToDictionary();
    }
}
