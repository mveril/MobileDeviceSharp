#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace MobileDeviceSharp.Native
{

    [CustomMarshaller(typeof(CustomMarshallerAttribute.GenericPlaceholder[]), MarshalMode.ManagedToUnmanagedIn, typeof(NullTerminatedArrayMarshaller<,>.ManagedToUnmanaged))]
    [CustomMarshaller(typeof(CustomMarshallerAttribute.GenericPlaceholder[]), MarshalMode.UnmanagedToManagedOut, typeof(NullTerminatedArrayMarshaller<,>.ManagedToUnmanaged))]
    [CustomMarshaller(typeof(CustomMarshallerAttribute.GenericPlaceholder[]), MarshalMode.ElementIn, typeof(NullTerminatedArrayMarshaller<,>.ManagedToUnmanaged))]
    [CustomMarshaller(typeof(CustomMarshallerAttribute.GenericPlaceholder[]), MarshalMode.UnmanagedToManagedIn, typeof(NullTerminatedArrayMarshaller<,>.UnmanagedToManaged))]
    [CustomMarshaller(typeof(CustomMarshallerAttribute.GenericPlaceholder[]), MarshalMode.ManagedToUnmanagedOut, typeof(NullTerminatedArrayMarshaller<,>.UnmanagedToManaged))]
    [ContiguousCollectionMarshaller]
    public unsafe static class NullTerminatedArrayMarshaller<T,Tunmanaged> where Tunmanaged : unmanaged
    {
        public static class ManagedToUnmanaged
        {
            public static Tunmanaged* AllocateContainerForUnmanagedElements(T[]? managed, out int numElements)
            {
                if (managed is null)
                {
                    numElements = 0;
                    return null;
                }

                numElements = managed.Length;
                var spaceToAllocate = numElements + 1;
                return (Tunmanaged*)Marshal.AllocCoTaskMem(spaceToAllocate).ToPointer();
            }

            public static ReadOnlySpan<T> GetManagedValuesSource(T[] managed) => managed;

            public static Span<Tunmanaged> GetUnmanagedValuesDestination(Tunmanaged* unmanaged, int numElements) => new Span<Tunmanaged>(unmanaged, numElements);

            public static void Free(IntPtr unmanaged)
            {
                Marshal.FreeCoTaskMem(unmanaged);
            }
        }

        public unsafe ref struct UnmanagedToManaged // Can be ref struct
        {
            private int _length;
            private T[] _array;
            private Tunmanaged* _unmanagedPointer;

            private int calculateMemoryAreaSize(Tunmanaged* unmanaged)
            {
                // Search for the null value (0) using a for loop
                Tunmanaged* startPtr = unmanaged;
                Tunmanaged* endPtr = unmanaged;

                for (endPtr = startPtr; endPtr is not null; endPtr++)
                {
                    // Nothing to do here, just advance the endPtr until we find the null.
                }

                // Calculate the size of the memory region without counting the null
                return (int)(endPtr - startPtr);
            }
            public void FromUnmanaged(Tunmanaged* value)
            {
                _unmanagedPointer = value;
                _length = calculateMemoryAreaSize(_unmanagedPointer);
                _array = new T[_length];
            }

            public ReadOnlySpan<Tunmanaged> GetUnmanagedValuesSource(int numElements) => GetUnmanagedValuesDestination();

            public Span<Tunmanaged> GetUnmanagedValuesDestination() => new Span<Tunmanaged>(_unmanagedPointer, _length);
            public Span<T> GetManagedValuesDestination(int numElements) => _array;

            public T[] ToManaged() => _array;

            public void Free()
            {
                
            }
        }
    }
}
#endif
