#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using static MobileDeviceSharp.Native.IDevice;

namespace MobileDeviceSharp.Native
{
    [CustomMarshaller(typeof(string[]), MarshalMode.Default, typeof(IDeviceListExtendedMarshaller))]
    [ContiguousCollectionMarshaller]
    public unsafe static class IDeviceListExtendedMarshaller
    {
        public static byte** ConvertToUnmanaged(string[] managed)
        {
            return NullTerminatedUTF8StringArrayMarshaller.ConvertToUnmanaged(managed);
        }

        public static string[] ConvertToManaged(byte** unmanaged)
        {
            return NullTerminatedUTF8StringArrayMarshaller.ConvertToManaged(unmanaged);
        }

        public static void Free(byte** unmanaged)
        {
            idevice_device_list_extended_free((IntPtr)unmanaged);
        }
    }
}
#endif
