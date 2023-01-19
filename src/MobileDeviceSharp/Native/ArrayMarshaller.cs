using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileDeviceSharp.Native
{
    public class ArrayMarshaller<T, TMarshaler> : CustomMashaler<T[]> where TMarshaler : CustomMashaler<T>
    {
        private readonly TMarshaler _itemMarshaler;

        private static readonly MethodInfo[] s_GetInstanceMethods = Array.FindAll(typeof(TMarshaler).GetMethods(BindingFlags.Static | BindingFlags.Public),x=> x.Name == "GetInstance");
        private static readonly Lazy<ArrayMarshaller<T, TMarshaler>> s_static_instance = new();

        private static TMarshaler GetMarshallerInstance()
        {
            return (TMarshaler)GetInstanceMethod(false).Invoke(null, Array.Empty<object>())!;
        }

        private static TMarshaler GetMarshallerInstance(string cookie)
        {
            return (TMarshaler)GetInstanceMethod(true).Invoke(null, new object[] { cookie })!;
        }

        private static MethodInfo GetInstanceMethod(bool withParameter)
        {
            foreach (var method in s_GetInstanceMethods)
            {
                var parameters = method.GetParameters();
#if NET6_0_OR_GREATER
                var count = parameters.TryGetNonEnumeratedCount(out var internalCount) ? internalCount : parameters.Count();
#else
                var count = parameters.Count();
#endif
                if(count == 0 || withParameter)
                {
                    return method;
                }
            }
            throw new NotSupportedException();
        }

        public ArrayMarshaller() : this(GetMarshallerInstance())
        {
            
        }

        public ArrayMarshaller(string cookie) : this(GetMarshallerInstance(cookie))
        {
        
        }

        public ArrayMarshaller(TMarshaler itemMarshaller)
        {
            _itemMarshaler = itemMarshaller;
        }

        public override unsafe IntPtr MarshalManagedToNative(T[] managedObj)
        {

            if (managedObj == null)
            {
                return IntPtr.Zero;
            }
            IntPtr pUnmanagedData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)) * managedObj.Length + 1);
            var UnmanagedData = new Span<IntPtr>(pUnmanagedData.ToPointer(), managedObj.Length + 1);

            for (int i = 0; i < managedObj.Length; i++)
            {
                UnmanagedData[i] = _itemMarshaler.MarshalManagedToNative(managedObj[i]);
            }

            UnmanagedData[managedObj.Length] = IntPtr.Zero;
            return pUnmanagedData;
        }

        public override unsafe T[] MarshalNativeToManaged(IntPtr pNativeData)
        {
            var list = new List<T>();
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
                    list.Add(_itemMarshaler.MarshalNativeToManaged(stringPointer));
                    arrayIndex++;
                }
                return list.ToArray();
            }
            return Array.Empty<T>();
        }
        public override unsafe void CleanUpNativeData(IntPtr pNativeData)
        {
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
                    _itemMarshaler.CleanUpNativeData(stringPointer);
                    arrayIndex++;
                }
            }

            Marshal.FreeHGlobal(pNativeData);
        }

        public override void CleanUpManagedData(T[] managedObj)
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

        public static ArrayMarshaller<T, TMarshaler> GetInstance()
        {
            return s_static_instance.Value;
        }
    }
}
