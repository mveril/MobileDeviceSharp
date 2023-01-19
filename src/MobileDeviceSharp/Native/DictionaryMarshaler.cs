using MobileDeviceSharp.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Reflection;

namespace MobileDeviceSharp.Native
{
    public class DictionaryMarshaler<T,TMarshaler> : CustomMashaler<IReadOnlyDictionary<T,T>> where TMarshaler : CustomMashaler<T>
    {

        private readonly ArrayMarshaller<T,TMarshaler> _arrayMarshaler;

        private static readonly Lazy<DictionaryMarshaler<T,TMarshaler>> s_static_instance = new();

        public DictionaryMarshaler() : this(ArrayMarshaller<T,TMarshaler>.GetInstance())
        {

        }

        public DictionaryMarshaler(string cookie) : this((ArrayMarshaller<T, TMarshaler>)ArrayMarshaller<T, TMarshaler>.GetInstance(cookie))
        {

        }

        public DictionaryMarshaler(ArrayMarshaller<T,TMarshaler> arrayMarshaler)
        {
            _arrayMarshaler = arrayMarshaler;
        }

        public DictionaryMarshaler(TMarshaler itemMarshaler) : this(new ArrayMarshaller<T,TMarshaler>(itemMarshaler))
        {

        }

        public override void CleanUpManagedData(IReadOnlyDictionary<T, T> managedObj)
        {

        }

        public override void CleanUpNativeData(IntPtr pNativeData)
        {
            _arrayMarshaler.CleanUpNativeData(pNativeData);
        }

        public override int GetNativeDataSize()
        {
            return _arrayMarshaler.GetNativeDataSize();
        }

        public override IntPtr MarshalManagedToNative(IReadOnlyDictionary<T,T> managedObj)
        {
            if (managedObj == null)
            {
                return IntPtr.Zero;
            }

            var array = managedObj.SelectMany((kv) => new[] { kv.Key, kv.Value }).ToArray();
            return _arrayMarshaler.MarshalManagedToNative(array);
        }

        public override IReadOnlyDictionary<T,T> MarshalNativeToManaged(IntPtr pNativeData)
        {
            var array = _arrayMarshaler.MarshalNativeToManaged(pNativeData).AsEnumerable();
            var dic = new Dictionary<T,T>();
            var enumerator = array.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T key, value;
                key = enumerator.Current;
                if (!enumerator.MoveNext())
                {
                    break;
                }
                value = enumerator.Current;
                dic.Add(key, value);
            }
             return new ReadOnlyDictionary<T, T>(dic);
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return s_static_instance.Value;
        }

        public static DictionaryMarshaler<T,TMarshaler> GetInstance()
        {
            return s_static_instance.Value;
        }
    }
}
