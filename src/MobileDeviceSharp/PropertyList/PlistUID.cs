using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList
{
    /// <summary>
    /// Represent a plist node that contain <see cref="long"/>.
    /// </summary>
    public sealed class PlistUid : PlistValueNode<long>
    {
        /// <summary>
        /// Create <see cref="long"/> plist node from an existing handle.
        /// </summary>
        /// <param name="handle">The <see cref="PlistHandle"/> of type <see cref="PlistType.Uid"/> to wrap.</param>
        public PlistUid(PlistHandle handle) : base(handle)
        {

        }

        /// <summary>
        /// Create <see cref="long"/> plist node.
        /// </summary>
        public PlistUid() : this(default(long))
        {

        }

        /// <summary>
        /// Create <see cref="long"/> plist node from a value.
        /// </summary>
        /// <param name="value">the <see cref="ulong"/> value.</param>
        public PlistUid(long value) : base(plist_new_uid(value))
        {

        }

        /// <inheritdoc/>
        public override long Value
        {
            get
            {
                plist_get_uid_val(Handle, out long val);
                return val;
            }
            set => plist_set_uid_val(Handle, value);
        }
    }
}

