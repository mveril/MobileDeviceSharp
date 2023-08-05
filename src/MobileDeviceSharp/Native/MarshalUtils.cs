#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MobileDeviceSharp.Native
{
    public unsafe static class MarshalUtils
    {
        public static int GetNullTerminatedSize<T>(T* ptr) where T : struct
        {
            var i = 0;
            while (!ptr[i].Equals(default(T)))
            {
                i++;
            }
            return i;
        }

        public static Span<T> GetNullTerminatedSpan<T>(T* ptr) where T : struct
        {
            return new Span<T>(ptr, GetNullTerminatedSize(ptr));
        }

        public static ReadOnlySpan<T> GetNullTerminatedReadOnlySpan<T>(T* ptr) where T : struct
        {
            return new ReadOnlySpan<T>(ptr, GetNullTerminatedSize(ptr));
        }
    }
}
#endif
