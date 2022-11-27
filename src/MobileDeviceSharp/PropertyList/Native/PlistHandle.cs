using System;
using System.Collections.Generic;
using System.Text;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList.Native
{
    public sealed partial class PlistHandle 
    {
        protected override bool CanBeReleased()
        {
            return plist_get_parent(this) == Zero;
        }
    }
}
