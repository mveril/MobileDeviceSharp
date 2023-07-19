using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.Native;

namespace MobileDeviceSharp.PropertyList.Native
{
    public partial class PlistHandle : IOSHandle
    {
        internal PlistHandle(IntPtr handle, bool ownsHandle) : base(handle, ownsHandle)
        {

        }
    }
}
