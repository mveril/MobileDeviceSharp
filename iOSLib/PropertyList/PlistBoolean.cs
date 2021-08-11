using System;
using System.Collections.Generic;
using System.Text;
using IOSLib.PropertyList.Native;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    public sealed class PlistBoolean : PlistValueNode<bool>
    {
        public PlistBoolean(PlistHandle handle) : base(handle)
        {

        }

        public PlistBoolean(bool Value) : base(plist_new_bool(Value))
        {

        }

        public override bool Value
        {
            get
            {
                plist_get_bool_val(Handle, out var val);
                return val != 0;
            }
            set => plist_set_bool_val(Handle, value ? (byte)1 : (byte)0);
        }
    }
}
