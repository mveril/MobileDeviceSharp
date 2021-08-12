using System;
using System.Collections.Generic;
using System.Text;
using IOSLib.PropertyList.Native;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    public sealed class PlistData : PlistValueNode<byte[]>
    {
        public PlistData(PlistHandle handle) : base(handle)
        {

        }

        public PlistData(byte[] value) : base(Create(value))
        {

        }

        private static PlistHandle Create(byte[] value)
        {
            return plist_new_data(value, (uint)value.Length);
        }

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

