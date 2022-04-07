using System;
using System.Collections.Generic;
using System.Text;
using IOSLib.PropertyList.Native;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    /// <summary>
    /// Represent a plist node that contain bytes data.
    /// </summary>
    public sealed class PlistData : PlistValueNode<byte[]>
    {
        /// <summary>
        /// Create data plist node from an existing handle.
        /// </summary>
        /// <param name="handle">The <see cref="PlistHandle"/> of type <see cref="PlistType.Data"/> to wrap.</param>
        public PlistData(PlistHandle handle) : base(handle)
        {

        }

        /// <summary>
        /// Initialize new <see cref="PlistData"/> from a byte array.
        /// </summary>
        /// <param name="value">The data.</param>
        public PlistData(byte[] value) : base(Create(value))
        {

        }

        private static PlistHandle Create(byte[] value)
        {
            return plist_new_data(value, (uint)value.Length);
        }
        /// <summary>
        /// Initialize new <see cref="PlistData"/> from a <see cref="ReadOnlySpan{Byte}"/>.
        /// </summary>
        /// <param name="value">The data.</param>
        public PlistData(ReadOnlySpan<byte> value) : base(Create(value))
        {

        }

        private unsafe static PlistHandle Create(ReadOnlySpan<byte> value)
        {
            fixed(byte* ptr = value)
            {
                return plist_new_data(ptr, (uint)value.Length);
            }
        }

        public override byte[] Value
        {
            get
            {
                plist_get_data_val(Handle, out var val, out _);
                return val;
            }
            set => plist_set_data_val(Handle,value, (ulong)value.Length);
        }
    }
}

