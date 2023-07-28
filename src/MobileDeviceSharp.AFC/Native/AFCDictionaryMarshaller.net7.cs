#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using static MobileDeviceSharp.AFC.Native.AFC;
using System.Text;
using MobileDeviceSharp.AFC.Native;
using System.Collections.ObjectModel;

namespace MobileDeviceSharp.Native
{
    [CustomMarshaller(typeof(ReadOnlyStringDictionary), MarshalMode.Default, typeof(AFCDictionaryMarshaller))]
    internal static unsafe class AFCDictionaryMarshaller
    {
        public static byte** ConvertToUnmanaged(ReadOnlyStringDictionary managed)
        {
            return ReadOnlyStringDictionaryMarshaller.ConvertToUnmanaged(managed);
        }

        public static ReadOnlyStringDictionary ConvertToManaged(byte** unmanaged)
        {
            return ReadOnlyStringDictionaryMarshaller.ConvertToManaged(unmanaged);
        }
        public static void Free(byte** unmanaged)
        {
            afc_dictionary_free((IntPtr)unmanaged);
        }
    }
}
#endif
