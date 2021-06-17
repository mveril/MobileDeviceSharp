using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace IDevice.NET.Core.Native
{
    public class UTF8Marshaler : ICustomMarshaler
    {
        static Lazy<UTF8Marshaler> static_instance = new Lazy<UTF8Marshaler>();

        public unsafe IntPtr MarshalManagedToNative(object managedObj)
        {
            if (managedObj == null)
                return IntPtr.Zero;
            if (!(managedObj is string))
                throw new MarshalDirectiveException(
                       "UTF8Marshaler must be used on a string.");

            // not null terminated
            ReadOnlySpan<byte> strbuf = Encoding.UTF8.GetBytes((string)managedObj);
            var buffer = Marshal.AllocHGlobal(strbuf.Length + 1);
            var uspan = new Span<byte>(buffer.ToPointer(),strbuf.Length+1);
            strbuf.CopyTo(uspan);
            uspan[strbuf.Length] = 0;
            return buffer;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
#if NETSTANDARD2_0
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
#else
            return Marshal.PtrToStringUTF8(pNativeData);
#endif
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        public void CleanUpManagedData(object managedObj)
        {
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public static ICustomMarshaler GetInstance()
        {
           return static_instance.Value;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return static_instance.Value;
        }
    }
}
