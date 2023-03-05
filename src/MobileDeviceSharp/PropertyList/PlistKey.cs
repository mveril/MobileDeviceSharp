using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList
{
    /// <summary>
    /// Represent a key to a plistNode
    /// </summary>
    public sealed class PlistKey : PlistValueNode<string>
    {
        /// <summary>
        /// Create plist key node from an existing handle.
        /// </summary>
        /// <param name="handle">The <see cref="PlistHandle"/> of type <see cref="PlistType.Key"/> to wrap.</param>
        public PlistKey(PlistHandle handle) : base(handle)
        {

        }

        /// <inheritdoc/>
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

