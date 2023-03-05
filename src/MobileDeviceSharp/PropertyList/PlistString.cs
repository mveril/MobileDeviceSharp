using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList
{
    /// <summary>
    /// Represent a plist node which contain a <see cref="string"/> value.
    /// </summary>
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
        /// <param name="value">The <see cref="string"/> value.</param>
        public PlistString(string value) : base(plist_new_string(value))
        {

        }

        /// <summary>
        /// Create <see cref="string"/> plist node from a <see cref="ReadOnlySpan{Char}"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> value as <see cref="ReadOnlySpan{Char}"/>.</param>
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

        /// <summary>
        /// Get a <see cref="TextReader"/> in order to have access to the underlying string without copying it.
        /// </summary>
        /// <returns>A <see cref="TextReader"/> that wrap the string containing in this <see cref="PlistString"/></returns>
        public TextReader GetReader()
        {
            unsafe
            {
                var ptr = (byte*)plist_get_string_ptr(Handle, out var length);
                var stream = new UnmanagedMemoryStream(ptr, (long)length,(long)length,FileAccess.Read);
                return new StreamReader(stream, Encoding.UTF8);
            }
        }
    }
}

