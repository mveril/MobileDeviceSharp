using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
namespace IOSLib.Native
{
    public class UTF8ArrayMarshaler : CustomMashaler<string[]>
    {
        private static readonly Lazy<UTF8ArrayMarshaler> s_static_instance = new();

        public override unsafe IntPtr MarshalManagedToNative(string[] managedObj)
        {
            var stringMarshaler = UTF8Marshaler.GetInstance();

            if (managedObj == null)
            {
                return IntPtr.Zero;
            }
            IntPtr pUnmanagedData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)) * managedObj.Length + 1);
            var UnmanagedData = new Span<IntPtr>(pUnmanagedData.ToPointer(), managedObj.Length + 1);

            for (int i = 0; i < managedObj.Length; i++)
            {
                UnmanagedData[i] = stringMarshaler.MarshalManagedToNative(managedObj[i]);
            }

            UnmanagedData[managedObj.Length] = IntPtr.Zero;
            return pUnmanagedData;
        }

        public override unsafe string[] MarshalNativeToManaged(IntPtr pNativeData)
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
        public override unsafe void CleanUpNativeData(IntPtr pNativeData)
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

        public override void CleanUpManagedData(string[] managedObj)
        {

        }

        public override int GetNativeDataSize()
        {
            return -1;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return s_static_instance.Value;
        }

        public static UTF8ArrayMarshaler GetInstance()
        {
            return s_static_instance.Value;
        }
    }
}
