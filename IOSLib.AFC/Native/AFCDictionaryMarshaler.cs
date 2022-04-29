using IOSLib.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IOSLib.AFC.Native
{
    class AFCDictionaryMarshaler : UTF8DictionaryMarshaler
    {

        private static readonly Lazy<AFCDictionaryMarshaler> s_static_instance = new();

        public override void CleanUpNativeData(IntPtr pNativeData)
        {
            AFC.afc_dictionary_free(pNativeData);
        }

        public static new ICustomMarshaler GetInstance(string cookie)
        {
            return s_static_instance.Value;
        }

        public static new AFCDictionaryMarshaler GetInstance()
        {
            return s_static_instance.Value;
        }
    }
}
