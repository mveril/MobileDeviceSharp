using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.PropertyList.Native
{
    public partial class PlistHandle
    {
        internal PlistHandle(IntPtr handle, bool ownsHandle) : base(handle, ownsHandle)
        {

        }
    }
}
