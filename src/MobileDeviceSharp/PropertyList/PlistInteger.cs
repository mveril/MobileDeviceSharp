using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList
{
    /// <summary>
    /// Represent a plist node that contain an <see cref="ulong"/> value
    /// </summary>
    public sealed class PlistInteger : PlistValueNode<ulong>
    {
        /// <summary>
        /// Create <see cref="ulong"/> plist node from an existing handle.
        /// </summary>
        /// <param name="handle">The <see cref="PlistHandle"/> of type <see cref="PlistType.Uint"/> to wrap.</param>
        public PlistInteger(PlistHandle handle) : base(handle)
        {

        }

        /// <summary>
        /// Create <see cref="ulong"/> plist node from a value.
        /// </summary>
        /// <param name="Value">the <see cref="ulong"/> value.</param>
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
