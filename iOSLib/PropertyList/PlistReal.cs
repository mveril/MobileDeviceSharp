using System;
using System.Collections.Generic;
using System.Text;
using IOSLib.PropertyList.Native;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    public sealed class PlistReal : PlistValueNode<double>
    {
        public PlistReal(PlistHandle handle) : base(handle)
        {

        }

        public PlistReal(double value) : base(plist_new_real(value))
        {

        }

        public override double Value
        {
            get
            {
                plist_get_real_val(Handle, out var val);
                return val;
            }
            set => plist_set_real_val(Handle, value);
        }
    }
}

