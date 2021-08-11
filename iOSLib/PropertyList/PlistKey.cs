using System;
using System.Collections.Generic;
using System.Text;
using IOSLib.PropertyList.Native;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    public sealed class PlistKey : PlistValueNode<string>
    {
        public PlistKey(PlistHandle handle) : base(handle)
        {

        }


        public override string Value
        {
            get
            {
                plist_get_key_val(Handle, out string val);
                return val;
            }
            set => plist_set_key_val(Handle, value);
        }
    }
}

