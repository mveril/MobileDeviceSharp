#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Text.Unicode;
using MobileDeviceSharp.AFC.Native;

namespace MobileDeviceSharp.Native
{


    [CustomMarshaller(typeof(string[]),MarshalMode.Default, typeof(NullTerminatedUTF8StringDecomposedArrayMarshaller))]
    public unsafe static class NullTerminatedUTF8StringDecomposedArrayMarshaller
    {

        public static byte** ConvertToUnmanaged(string[] managed)
        {
            var unmanaged = (byte**)Marshal.AllocCoTaskMem(managed.Length*IntPtr.Size);
            var unmanagedView = new Span<IntPtr>(unmanaged, managed.Length);
            for (int i = 0; i < managed.Length; i++)
            {
                unmanagedView[i] = (IntPtr)UTF8StringDecomposedMarshaller.ConvertToUnmanaged(managed[i]);
            }
            return unmanaged;
        }

        public static string[] ConvertToManaged(byte** unmanaged)
        {
            ReadOnlySpan<IntPtr> unmanagedArray = MarshalUtils.GetNullTerminatedReadOnlySpan((IntPtr*)unmanaged);
            var result = new string[unmanagedArray.Length];
            for (int i = 0; i < unmanagedArray.Length; i++)
            {
                result[i] = UTF8StringDecomposedMarshaller.ConvertToManaged((byte*)unmanagedArray[i]);
            }
            return result;

        }

        public static void Free(byte** unmanaged)
        {
            var ptr = unmanaged;
            while (*ptr != null)
            {
                UTF8StringDecomposedMarshaller.Free(*ptr);
                ptr++;
            }
            Marshal.FreeCoTaskMem((IntPtr)unmanaged);
        }
    }
}
#endif
