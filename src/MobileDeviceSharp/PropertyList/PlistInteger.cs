using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList
{
    /// <summary>
    /// Represent a plist node that contain an <see cref="long"/> value
    /// </summary>
    public sealed class PlistInteger : PlistValueNode<long>
    {
        /// <summary>
        /// Create <see cref="long"/> plist node from an existing handle.
        /// </summary>
        /// <param name="handle">The <see cref="PlistHandle"/> of type <see cref="PlistType.Uint"/> to wrap.</param>
        public PlistInteger(PlistHandle handle) : base(handle)
        {

        }

        /// <summary>
        /// Create <see cref="long"/> plist node.
        /// </summary>
        public PlistInteger(): this(default(int))
        {

        }

        /// <summary>
        /// Create <see cref="long"/> plist node from a value.
        /// </summary>
        /// <param name="Value">the <see cref="long"/> value.</param>
        public PlistInteger(long Value) : base(plist_new_uint(Value))
        {

        }

        public override long Value
        {
            get
            {
                plist_get_uint_val(Handle, out long value);
                return value;
            }
            set => plist_set_uint_val(Handle, value);
        }
    }
}
