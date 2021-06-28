using IOSLib.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;

namespace IOSLib.Native
{
    public class UTF8DictionaryMarshaler : ICustomMarshaler
    {
        static Lazy<UTF8DictionaryMarshaler> static_instance = new Lazy<UTF8DictionaryMarshaler>();
        public void CleanUpManagedData(object ManagedObj)
        {
            
        }

        public virtual void CleanUpNativeData(IntPtr pNativeData)
        {
            UTF8ArrayMarshaler.GetInstance().CleanUpManagedData(pNativeData);
        }

        public int GetNativeDataSize()
        {
            return UTF8ArrayMarshaler.GetInstance().GetNativeDataSize();
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            var stringMarshaler = UTF8Marshaler.GetInstance();
            var dic = ManagedObj as IDictionary<string, string>;

            if (dic == null)
            {
                return IntPtr.Zero;
            }
            var array = dic.SelectMany((kv) => new[] { kv.Key, kv.Value });
            return UTF8ArrayMarshaler.GetInstance().MarshalManagedToNative(array);
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            var array = (string[])UTF8ArrayMarshaler.GetInstance().MarshalNativeToManaged(pNativeData);
            var dic = new Dictionary<string, string>();
                var enumerator = array.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    string key, value;
                    key = (string)enumerator.Current;
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }                    
                    value = (string)enumerator.Current;
                    dic.Add(key, value);
                }
                return new ReadOnlyDictionary<string, string>(dic);
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
