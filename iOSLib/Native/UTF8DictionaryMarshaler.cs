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
        static Lazy<UTF8DictionaryMarshaler> static_instance = new Lazy<UTF8DictionaryMarshaler>();
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
            var array = UTF8ArrayMarshaler.GetInstance().MarshalNativeToManaged(pNativeData);
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
