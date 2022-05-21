using System;
using System.Collections.Generic;
using System.Text;
using IOSLib.PropertyList.Native;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    public sealed class PlistReal : PlistValueNode<double>
    {
        /// <summary>
        /// Create <see cref="double"/> plist node from an existing handle.
        /// </summary>
        /// <param name="handle">The <see cref="PlistHandle"/> of type <see cref="PlistType.Real"/> to wrap.</param>
        public PlistReal(PlistHandle handle) : base(handle)
        {

        }
        /// <summary>
        /// Create <see cref="double"/> plist node from a value.
        /// </summary>
        /// <param name="value">the <see cref="double"/> value.</param>
        public PlistReal(double value) : base(plist_new_real(value))
        {

        }

        public override double Value
        {
            get
            {
                plist_get_real_val(Handle, out var val);
                return val;
            }
            set => plist_set_real_val(Handle, value);
        }
    }
}

