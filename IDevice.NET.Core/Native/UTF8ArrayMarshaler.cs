using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
namespace IDevice.NET.Core.Native
{
    public class UTF8ArrayMarshaler : ICustomMarshaler
    {
        static Lazy<UTF8ArrayMarshaler> static_instance = new Lazy<UTF8ArrayMarshaler>();

        public virtual unsafe IntPtr MarshalManagedToNative(object managedObj)
        {
            var stringMarshaler = UTF8Marshaler.GetInstance();
            var values = managedObj as string[];

            if (values == null)
            {
                return IntPtr.Zero;
            }
            IntPtr pUnmanagedData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)) * values.Length + 1);
            var UnmanagedData = new Span<IntPtr>(pUnmanagedData.ToPointer(), values.Length + 1);

            for (int i = 0; i < values.Length; i++)
            {
                UnmanagedData[i] = stringMarshaler.MarshalManagedToNative(values[i]);
            }

            UnmanagedData[values.Length] = IntPtr.Zero;
            return pUnmanagedData;
        }

        public virtual unsafe object MarshalNativeToManaged(IntPtr pNativeData)
        {
            var list = new List<string>();
            var stringMarshaler = UTF8Marshaler.GetInstance();
            if (pNativeData != IntPtr.Zero)
            {
                IntPtr* arrayIndex = (IntPtr*)pNativeData;
                while (true)
                {
                    IntPtr stringPointer = *arrayIndex;

                    if (stringPointer == IntPtr.Zero)
                    {
                        break;
                    }
                    list.Add((string)stringMarshaler.MarshalNativeToManaged(stringPointer));
                    arrayIndex++;
                }
                return list.ToArray();
            }
            return Array.Empty<string>();
        }
        public virtual unsafe void CleanUpNativeData(IntPtr pNativeData)
        {
            var stringMarshaler = UTF8Marshaler.GetInstance();
            if (pNativeData != IntPtr.Zero)
            {
                IntPtr* arrayIndex = (IntPtr*)pNativeData;

                while (true)
                {
                    IntPtr stringPointer = *arrayIndex;

                    if (stringPointer == IntPtr.Zero)
                    {
                        break;
                    }
                    stringMarshaler.CleanUpNativeData(stringPointer);
                    arrayIndex ++;
                }
            }

            Marshal.FreeHGlobal(pNativeData);
        }

        public virtual void CleanUpManagedData(object managedObj)
        {

        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return static_instance.Value;
        }

        public static ICustomMarshaler GetInstance()
        {
            return static_instance.Value;
        }
    }
}
