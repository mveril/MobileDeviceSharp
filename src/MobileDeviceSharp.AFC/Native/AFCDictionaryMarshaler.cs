#if !NET7_0_OR_GREATER
using MobileDeviceSharp.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileDeviceSharp.AFC.Native
{
    class AFCDictionaryMarshaler : DictionaryMarshaler<string, UTF8Marshaler>
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
#endif
