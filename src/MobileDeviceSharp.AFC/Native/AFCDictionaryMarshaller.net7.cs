#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using static MobileDeviceSharp.AFC.Native.AFC;
using System.Text;
using MobileDeviceSharp.AFC.Native;

namespace MobileDeviceSharp.Native
{
    [ContiguousCollectionMarshaller]
    [CustomMarshaller(typeof(SameKeyValueTypeReadonlyDictionary<>), MarshalMode.ManagedToUnmanagedOut, typeof(AFCDictionaryMarshaller<,>.UnmanagedToManaged))]
    [CustomMarshaller(typeof(SameKeyValueTypeReadonlyDictionary<>), MarshalMode.UnmanagedToManagedIn, typeof(AFCDictionaryMarshaller<,>.UnmanagedToManaged))]
    internal static unsafe class AFCDictionaryMarshaller<T, TUnmanagedElement> where T : notnull where TUnmanagedElement : unmanaged
    {
        public ref struct UnmanagedToManaged
        {
            NullTerminatedReadonlyDictionaryMarshaller<T, TUnmanagedElement>.UnmanagedToManaged _dicMarshaller = new();
            private TUnmanagedElement* _dict_pointer;

            public UnmanagedToManaged()
            {

            }

            public void FromUnmanaged(TUnmanagedElement* value)
            {
                _dict_pointer = value;
                _dicMarshaller.FromUnmanaged(value);
            }
            
            public ReadOnlySpan<TUnmanagedElement> GetUnmanagedValuesSource(int numElements) => _dicMarshaller.GetUnmanagedValuesSource(numElements);

            public Span<T?> GetManagedValuesDestination(int numElements) => _dicMarshaller.GetManagedValuesDestination(numElements);

            public SameKeyValueTypeReadonlyDictionary<T> ToManagedFinally() => _dicMarshaller.ToManagedFinally();

            public Span<TUnmanagedElement> GetUnmanagedValuesDestination() => _dicMarshaller.GetUnmanagedValuesDestination();

            public void Free()
            {
                afc_dictionary_free((IntPtr)_dict_pointer);
            }
        }
    }
}
#endif
