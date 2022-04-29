using IOSLib.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;

namespace IOSLib.Native
{
    public class UTF8DictionaryMarshaler : CustomMashaler<IReadOnlyDictionary<string,string>>
    {
        private static readonly Lazy<UTF8DictionaryMarshaler> s_static_instance = new();
        public override void CleanUpManagedData(IReadOnlyDictionary<string, string> ManagedObj)
        {
            
        }

        public override void CleanUpNativeData(IntPtr pNativeData)
        {
            UTF8ArrayMarshaler.GetInstance().CleanUpNativeData(pNativeData);
        }

        public override int GetNativeDataSize()
        {
            return UTF8ArrayMarshaler.GetInstance().GetNativeDataSize();
        }

        public override IntPtr MarshalManagedToNative(IReadOnlyDictionary<string,string> ManagedObj)
        {
            var stringMarshaler = UTF8Marshaler.GetInstance();

            if (ManagedObj == null)
            {
                return IntPtr.Zero;
            }

            var array = ManagedObj.SelectMany((kv) => new[] { kv.Key, kv.Value }).ToArray();
            return UTF8ArrayMarshaler.GetInstance().MarshalManagedToNative(array);
        }

        public override IReadOnlyDictionary<string,string> MarshalNativeToManaged(IntPtr pNativeData)
        {
            var array = UTF8ArrayMarshaler.GetInstance().MarshalNativeToManaged(pNativeData).AsEnumerable();
            var dic = new Dictionary<string, string>();
            var enumerator = array.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string key, value;
                key = enumerator.Current;
                if (!enumerator.MoveNext())
                {
                    break;
                }                    
                value = enumerator.Current;
                dic.Add(key, value);
            }
             return new ReadOnlyDictionary<string, string>(dic);
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return s_static_instance.Value;
        }

        public static UTF8DictionaryMarshaler GetInstance()
        {
            return s_static_instance.Value;
        }
    }
}
