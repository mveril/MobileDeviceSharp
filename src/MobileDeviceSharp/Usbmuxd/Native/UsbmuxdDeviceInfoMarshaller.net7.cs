#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace MobileDeviceSharp.Usbmuxd.Native
{
    [CustomMarshaller(typeof(UsbmuxdDeviceInfo), MarshalMode.Default, typeof(UsbmuxdDeviceInfoMarshaller))]
    internal static unsafe class UsbmuxdDeviceInfoMarshaller
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct UsbmuxdDeviceInfoNative
        {

            public uint handle;
            public uint product_id;
            public fixed byte udid[44];
            public int conn_type;
            public fixed byte conn_data[200];
        }

        public static IntPtr ConvertToUnmanaged(UsbmuxdDeviceInfo managed)
        {
            var result = new UsbmuxdDeviceInfoNative()
            {
                handle = managed.handle,
                product_id = managed.product_id,
                conn_type = (int)managed.conn_type,
            };
            var uuidSpan = new Span<byte>(result.udid, 44);
            Encoding.UTF8.GetBytes(managed.udid, uuidSpan);
            var connDataSpan = new Span<byte>(result.conn_data, 200);
            managed.conn_data.CopyTo(connDataSpan);
            var structPtr = Marshal.AllocCoTaskMem(sizeof(UsbmuxdDeviceInfoNative));
            Unsafe.Copy(structPtr.ToPointer(), ref result);
            return structPtr;
        }

        public static UsbmuxdDeviceInfo ConvertToManaged(IntPtr unmanaged)
        {
            var unsafePtr = (UsbmuxdDeviceInfoNative*)unmanaged;
            var value = new UsbmuxdDeviceInfo()
            {
                handle = unsafePtr->handle,
                product_id = unsafePtr->product_id,
                udid = Encoding.UTF8.GetString(unsafePtr->udid, 44),
                conn_type = (IDeviceLookupOptions)unsafePtr->conn_type,
                conn_data = new ReadOnlySpan<byte>(unsafePtr->conn_data, 200).ToArray(),
            };
            return value;
        }

        public static void Free(IntPtr unmanaged)
        {
            Marshal.FreeCoTaskMem(unmanaged);
        }
    }
}
#endif
