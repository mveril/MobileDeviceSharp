#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MobileDeviceSharp.Native
{
    public unsafe static class MarshalUtils
    {
        public static int GetNullOrDefaultTerminatedSize<T>(T** ptr) where T : unmanaged
        {
            var i = 0;
            while (ptr[i] != null && !(*ptr[i]).Equals(default(T)))
            {
                i++;
            }
            return i;
        }

        public static Span<IntPtr> GetNullOrDefaultTerminatedSpan<T>(T** ptr) where T: unmanaged
        {
            return new Span<IntPtr>(ptr, GetNullOrDefaultTerminatedSize(ptr));
        }

        public static ReadOnlySpan<IntPtr> GetNullOrDefaultTerminatedReadOnlySpan<T>(T** ptr) where T: unmanaged
        {
            return new ReadOnlySpan<IntPtr>(ptr, GetNullOrDefaultTerminatedSize(ptr));
        }

        public static int GetNullTerminatedSize(IntPtr* ptr)
        {
            var i = 0;
            while (ptr[i] != IntPtr.Zero)
            {
                i++;
            }
            return i;
        }

        public static Span<IntPtr> GetNullTerminatedSpan(IntPtr* ptr)
        {
            return new Span<IntPtr>(ptr, GetNullTerminatedSize(ptr));
        }

        public static ReadOnlySpan<IntPtr> GetNullTerminatedReadOnlySpan(IntPtr* ptr)
        {
            return new ReadOnlySpan<IntPtr>(ptr, GetNullTerminatedSize(ptr));
        }
    }
}
#endif
