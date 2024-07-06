using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.PropertyList;

namespace MobileDeviceSharp.InstallationProxy.AsyncAdapter
{
    internal record AsyncListProgress(ulong total, ulong currentIndex, ulong currentAmount, PlistArray list);
}
