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
        private const int UuidLength = 44;
        private const int ConnDataLength = 200;

        internal struct UsbmuxdDeviceInfoNative
        {

            public uint handle;
            public uint product_id;
            public fixed byte udid[UuidLength];
            public int conn_type;
            public fixed byte conn_data[ConnDataLength];
        }

        public static UsbmuxdDeviceInfoNative ConvertToUnmanaged(UsbmuxdDeviceInfo managed)
        {
            var result = new UsbmuxdDeviceInfoNative()
            {
                handle = managed.handle,
                product_id = managed.product_id,
                conn_type = (int)managed.conn_type,
            };
            var uuidSpan = new Span<byte>(result.udid, UuidLength);
            Encoding.UTF8.GetBytes(managed.udid, uuidSpan);
            var connDataSpan = new Span<byte>(result.conn_data, ConnDataLength);
            managed.conn_data.CopyTo(connDataSpan);
            return result;
        }

        public static UsbmuxdDeviceInfo ConvertToManaged(UsbmuxdDeviceInfoNative unmanaged)
        {
            var value = new UsbmuxdDeviceInfo()
            {
                handle = unmanaged.handle,
                product_id = unmanaged.product_id,
                udid = Encoding.UTF8.GetString(unmanaged.udid, UuidLength),
                conn_type = (IDeviceLookupOptions)unmanaged.conn_type,
                conn_data = new ReadOnlySpan<byte>(unmanaged.conn_data, ConnDataLength).ToArray(),
            };
            return value;
        }

        public static void Free(UsbmuxdDeviceInfoNative unmanaged)
        {

        }
    }
}
#endif
