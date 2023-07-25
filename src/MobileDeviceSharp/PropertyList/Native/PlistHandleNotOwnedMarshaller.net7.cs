#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace MobileDeviceSharp.PropertyList.Native
{
    [CustomMarshaller(typeof(PlistHandle), MarshalMode.ManagedToUnmanagedOut, typeof(PlistHandleNotOwnedMarshaller))]
    static class PlistHandleNotOwnedMarshaller
    {
        public static PlistHandle ConvertToManaged(IntPtr unmanaged)
        {
            return new PlistHandle(unmanaged, false);
        }
    }
}
#endif
