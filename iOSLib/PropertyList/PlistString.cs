using System;
using System.Collections.Generic;
using System.Text;
using IOSLib.PropertyList.Native;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    public sealed class PlistString : PlistValueNode<string>
    {
        /// <summary>
        /// Create <see cref="string"/> plist node from an existing handle.
        /// </summary>
        /// <param name="handle">The <see cref="PlistHandle"/> of type <see cref="PlistType.String"/> to wrap.</param>
        public PlistString(PlistHandle handle) : base(handle)
        {

        }

        /// <summary>
        /// Create <see cref="string"/> plist node from a value.
        /// </summary>
        /// <param name="Value">The <see cref="string"/> value.</param>
        public PlistString(string value) : base(plist_new_string(value))
        {

        }

        /// <summary>
        /// Create <see cref="string"/> plist node from a <see cref="ReadOnlySpan{Char}"/>.
        /// </summary>
        /// <param name="Value">The <see cref="string"/> value as <see cref="ReadOnlySpan{char}"/>.</param>
        public PlistString(ReadOnlySpan<char> value) : base(plist_new_string(new string(value.ToArray())))
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

