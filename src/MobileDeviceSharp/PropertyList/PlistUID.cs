using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList
{
    /// <summary>
    /// Represent a plist node that contain <see cref="ulong"/>.
    /// </summary>
    public sealed class PlistUid : PlistValueNode<ulong>
    {
        /// <summary>
        /// Create <see cref="ulong"/> plist node from an existing handle.
        /// </summary>
        /// <param name="handle">The <see cref="PlistHandle"/> of type <see cref="PlistType.Uid"/> to wrap.</param>
        public PlistUid(PlistHandle handle) : base(handle)
        {

        }

        /// <summary>
        /// Create <see cref="ulong"/> plist node from a value.
        /// </summary>
        /// <param name="Value">the <see cref="ulong"/> value.</param>
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

