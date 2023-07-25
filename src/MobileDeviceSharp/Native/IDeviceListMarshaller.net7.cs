#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using static MobileDeviceSharp.Native.IDevice;

namespace MobileDeviceSharp.Native
{
    [CustomMarshaller(typeof(CustomMarshallerAttribute.GenericPlaceholder[]), MarshalMode.UnmanagedToManagedIn, typeof(IDeviceListMarshaller<,>.UnmanagedToManaged))]
    [CustomMarshaller(typeof(CustomMarshallerAttribute.GenericPlaceholder[]), MarshalMode.ManagedToUnmanagedOut, typeof(IDeviceListMarshaller<,>.UnmanagedToManaged))]
    [ContiguousCollectionMarshaller]
    public unsafe static class IDeviceListMarshaller<T, Tunmanaged> where Tunmanaged : unmanaged
    {
        public unsafe ref struct UnmanagedToManaged // Can be ref struct
        {
            private readonly NullTerminatedArrayMarshaller<T, Tunmanaged>.UnmanagedToManaged _nullTerminatedArrayMarshaller = new();
            private Tunmanaged* _unmanagedPointer;

            public UnmanagedToManaged()
            {
            }

            public void FromUnmanaged(Tunmanaged* value)
            {
                _unmanagedPointer = value;
                _nullTerminatedArrayMarshaller.FromUnmanaged(value);
            }

            public ReadOnlySpan<Tunmanaged> GetUnmanagedValuesSource(int numElements) => _nullTerminatedArrayMarshaller.GetUnmanagedValuesSource(numElements);

            public Span<Tunmanaged> GetUnmanagedValuesDestination() => _nullTerminatedArrayMarshaller.GetUnmanagedValuesDestination();
            public Span<T> GetManagedValuesDestination(int numElements) => _nullTerminatedArrayMarshaller.GetManagedValuesDestination(numElements);

            public T[] ToManaged() => _nullTerminatedArrayMarshaller.ToManaged();

            public void Free()
            {
                idevice_device_list_free((IntPtr)_unmanagedPointer);
            }
        }
    }
}
#endif
