using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList
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

        private static unsafe PlistHandle Create(ReadOnlySpan<byte> value)
        {
            fixed(byte* ptr = value)
            {
                return plist_new_data(ptr, (uint)value.Length);
            }
        }

        /// <inheritdoc/>
        public override byte[] Value
        {
            get
            {
                plist_get_data_val(Handle, out var val, out _);
                return val;
            }
            set => plist_set_data_val(Handle,value, (ulong)value.Length);
        }

        /// <summary>
        /// Get a <see cref="ReadOnlySpan{Byte}"/> to have a readonly access to the data without copying them.
        /// </summary>
        /// <returns>A <see cref="ReadOnlySpan{Byte}"/> pointing to the underlying data.</returns>
        public ReadOnlySpan<byte> AsReadOnlySpan()
        {
            unsafe
            {
                var ptr = (byte*)plist_get_data_ptr(Handle, out var length);
                return new ReadOnlySpan<byte>(ptr, (int)length);
            }
        }

        public static implicit operator ReadOnlySpan<byte>(PlistData plist)
        {
            return plist.AsReadOnlySpan();
        }
    }
}

