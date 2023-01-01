using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList
{
    /// <summary>
    /// Represent a plist node that contain <see cref="Boolean"/>.
    /// </summary>
    public sealed class PlistBoolean : PlistValueNode<bool>
    {
        /// <summary>
        /// Create <see cref="Boolean"/> plist node from an existing handle.
        /// </summary>
        /// <param name="handle">The <see cref="PlistHandle"/> of type <see cref="PlistType.Boolean"/> to wrap.</param>
        public PlistBoolean(PlistHandle handle) : base(handle)
        {

        }

        /// <summary>
        /// Create <see cref="Boolean"/> plist node from a value.
        /// </summary>
        /// <param name="value">the <see cref="Boolean"/> value.</param>
        public PlistBoolean(bool value) : base(plist_new_bool(value))
        {

        }

        /// <inheritdoc/>
        public override bool Value
        {
            get
            {
                plist_get_bool_val(Handle, out var val);
                return val;
            }
            set => plist_set_bool_val(Handle, value);
        }
    }
}
