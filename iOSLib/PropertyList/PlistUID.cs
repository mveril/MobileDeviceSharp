using System;
using System.Collections.Generic;
using System.Text;
using IOSLib.PropertyList.Native;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    public sealed class PlistUid : PlistValueNode<ulong>
    {
        public PlistUid(PlistHandle handle) : base(handle)
        {

        }

        public PlistUid(ulong value) : base(plist_new_uid(value))
        {

        }

        public override ulong Value
        {
            get
            {
                plist_get_uid_val(Handle, out var val);
                return val;
            }
            set => plist_set_uid_val(Handle, value);
        }
    }
}

