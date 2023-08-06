using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.Native;

namespace MobileDeviceSharp.PropertyList.Native
{
    public partial class PlistHandle : IOSHandle
    {
        protected PlistHandle(bool ownsHandle) : base(ownsHandle)
        {

        }
    }
}
