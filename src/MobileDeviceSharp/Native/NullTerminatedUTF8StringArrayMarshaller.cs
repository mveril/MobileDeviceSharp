#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Text.Unicode;

namespace MobileDeviceSharp.Native
{


    [CustomMarshaller(typeof(string[]),MarshalMode.Default, typeof(NullTerminatedUTF8StringArrayMarshaller))]
    public unsafe static class NullTerminatedUTF8StringArrayMarshaller
    {

        public static byte** ConvertToUnmanaged(string[] managed)
        {
            var unmanaged = (byte**)Marshal.AllocCoTaskMem(managed.Length*IntPtr.Size);
            var unmanagedView = new Span<IntPtr>(unmanaged, managed.Length);
            for (int i = 0; i < managed.Length; i++)
            {
                unmanagedView[i] = (IntPtr)Utf8StringMarshaller.ConvertToUnmanaged(managed[i]);
            }
            return unmanaged;
        }

        public static string[] ConvertToManaged(byte** unmanaged)
        {
            ReadOnlySpan<IntPtr> unmanagedArray = MarshalUtils.GetNullTerminatedReadOnlySpan((IntPtr*)unmanaged);
            var result = new string[unmanagedArray.Length];
            for (int i = 0; i < unmanagedArray.Length; i++)
            {
                result[i] = Utf8StringMarshaller.ConvertToManaged((byte*)unmanagedArray[i]);
            }
            return result;

        }

        public static void Free(byte** unmanaged)
        {
            var ptr = unmanaged;
            while (*ptr != null)
            {
                Utf8StringMarshaller.Free(*ptr);
                ptr++;
            }
            Marshal.FreeCoTaskMem((IntPtr)unmanaged);
        }
    }
}
#endif
