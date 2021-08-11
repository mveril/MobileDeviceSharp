using System;
using System.Collections.Generic;
using System.Text;
using IOSLib.PropertyList.Native;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    public sealed class PlistString : PlistValueNode<string>
    {
        public PlistString(PlistHandle handle) : base(handle)
        {

        }

        public PlistString(string value) : base(plist_new_string(value))
        {

        }

        public override string Value
        {
            get
            {
                plist_get_string_val(Handle, out string val);
                return val;
            }
            set => plist_set_string_val(Handle, value);
        }
    }
}

