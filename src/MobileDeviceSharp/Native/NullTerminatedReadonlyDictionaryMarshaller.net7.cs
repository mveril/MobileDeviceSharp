#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace MobileDeviceSharp.Native
{

    [CustomMarshaller(typeof(SameKeyValueTypeReadonlyDictionary<>), MarshalMode.ManagedToUnmanagedOut, typeof(NullTerminatedReadonlyDictionaryMarshaller<,>.UnmanagedToManaged))]
    [CustomMarshaller(typeof(SameKeyValueTypeReadonlyDictionary<>), MarshalMode.UnmanagedToManagedIn, typeof(NullTerminatedReadonlyDictionaryMarshaller<,>.UnmanagedToManaged))]
    [ContiguousCollectionMarshaller]
    public static unsafe class NullTerminatedReadonlyDictionaryMarshaller<T, TUnmanagedElement> where T : notnull where TUnmanagedElement : unmanaged
    {
        public ref struct UnmanagedToManaged
        {
            NullTerminatedArrayMarshaller<T?, TUnmanagedElement>.UnmanagedToManaged _arrayMarshaller = new();

            public UnmanagedToManaged()
            {

            }

            public void FromUnmanaged(TUnmanagedElement* value)
            {
                _arrayMarshaller.FromUnmanaged(value);
            }

            public ReadOnlySpan<TUnmanagedElement> GetUnmanagedValuesSource(int numElements)
            {
                return _arrayMarshaller.GetUnmanagedValuesSource(numElements);
            }

            public Span<T?> GetManagedValuesDestination(int numElements)
            {
                return _arrayMarshaller.GetManagedValuesDestination(numElements);
            }

            public SameKeyValueTypeReadonlyDictionary<T> ToManagedFinally()
            {
                var array = _arrayMarshaller.ToManaged();
                var dic = new Dictionary<T, T?>();
                var it = array.AsEnumerable().GetEnumerator();
                while (it.MoveNext())
                {
                    var key = it.Current;
                    if (key is null || !it.MoveNext())
                    {
                        throw new NotSupportedException();
                    }
                    var value = it.Current;
                    dic.Add(key, value);
                }
                return new SameKeyValueTypeReadonlyDictionary<T>(dic);
            }

            public Span<TUnmanagedElement> GetUnmanagedValuesDestination()
            {
                return _arrayMarshaller.GetUnmanagedValuesDestination();
            }

            public void Free()
            {
                _arrayMarshaller.Free();
            }
        }
    }
}
#endif
