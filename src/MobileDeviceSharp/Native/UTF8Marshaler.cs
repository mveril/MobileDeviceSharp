using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileDeviceSharp.Native
{
    /// <summary>
    /// A marshaller for UTF8 string.
    /// </summary>
    public class UTF8Marshaler : CustomMashaler<string>
    {
        private static readonly Lazy<UTF8Marshaler> s_static_instance = new();

        /// <inheritdoc/>
        public override unsafe IntPtr MarshalManagedToNative(string managedObj)
        {
            // managedObj is not null terminated
            int nb = Encoding.UTF8.GetMaxByteCount(managedObj.Length);

            IntPtr ptr = Marshal.AllocHGlobal(nb + 1);

            int nbWritten;
            byte* pbMem = (byte*)ptr;

            fixed (char* firstChar = managedObj)
            {
                nbWritten = Encoding.UTF8.GetBytes(firstChar, managedObj.Length, pbMem, nb);
            }

            pbMem[nbWritten] = 0;

            return ptr;
        }

        /// <inheritdoc/>
        public override string MarshalNativeToManaged(IntPtr pNativeData)
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP1_1_OR_GREATER
            return Marshal.PtrToStringUTF8(pNativeData)!;
#else
            unsafe
            {
                byte* walk = (byte*)pNativeData;

                // find the end of the string
                while (*walk != 0)
                {
                    walk++;
                }
                int length = (int)(walk - (byte*)pNativeData);

                // should not be null terminated
                // skip the trailing null
                string data = Encoding.UTF8.GetString((byte*)pNativeData, length);
                return data;
            }
#endif
        }

        /// <inheritdoc/>
        public override void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        /// <inheritdoc/>
        public override void CleanUpManagedData(string managedObj)
        {
        }

        /// <inheritdoc/>
        public override int GetNativeDataSize()
        {
            return -1;
        }

        public static UTF8Marshaler GetInstance()
        {
           return s_static_instance.Value;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return s_static_instance.Value;
        }
    }
}
