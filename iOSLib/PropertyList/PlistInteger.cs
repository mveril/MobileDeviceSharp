using System;
using System.Collections.Generic;
using System.Text;
using IOSLib.PropertyList.Native;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    public sealed class PlistInteger : PlistValueNode<ulong>
    {
        public PlistInteger(PlistHandle handle) : base(handle)
        {

        }

        public PlistInteger(ulong Value) : base(plist_new_uint(Value))
        {

        }

        public override ulong Value
        {
            get
            {
                plist_get_uint_val(Handle, out var val);
                return val;
            }
            set => plist_set_uint_val(Handle, value);
        }
    }
}
