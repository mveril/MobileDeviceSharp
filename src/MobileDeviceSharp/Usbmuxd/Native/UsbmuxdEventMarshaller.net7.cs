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
    [CustomMarshaller(typeof(UsbmuxdEvent),MarshalMode.Default,typeof(UsbmuxdEventMarshaller))]
    internal static unsafe class UsbmuxdEventMarshaller
    {

        private const int SIZE_OF_USBMUXD_DEVICE_INFO = 256;

        [StructLayoutAttribute(LayoutKind.Sequential)]
        internal struct UsbmuxdEventNative
        {

            public int @event;

            public fixed byte device[SIZE_OF_USBMUXD_DEVICE_INFO];// sizeof(sizeof(UsbmuxdDeviceInfoMarshaller.UsbmuxdDeviceInfoNative)
        }

#if DEBUG
        static UsbmuxdEventMarshaller()
        {
            Debug.Assert(sizeof(UsbmuxdDeviceInfoMarshaller.UsbmuxdDeviceInfoNative) == SIZE_OF_USBMUXD_DEVICE_INFO);
        }
#endif

        public static nint ConvertToUnmanaged(UsbmuxdEvent managed)
        {
            var ptr = Marshal.AllocCoTaskMem(sizeof(UsbmuxdEventType));
            var unmanaged = new UsbmuxdEventNative()
            {
                @event = (int)managed.@event,
            };
            var deviceNativePtr = UsbmuxdDeviceInfoMarshaller.ConvertToUnmanaged(managed.device);
            var deviceNative = Unsafe.AsRef(deviceNativePtr);
            Unsafe.Copy(ref deviceNative, unmanaged.device);
            UsbmuxdDeviceInfoMarshaller.Free(deviceNativePtr);
            Unsafe.Copy(ref unmanaged, (UsbmuxdEventNative*)ptr);
            return ptr;
        }

        public static UsbmuxdEvent ConvertToManaged(nint unmanaged)
        {
            var ptr = (UsbmuxdEventNative*)unmanaged;
            var device = Unsafe.Read<UsbmuxdDeviceInfoMarshaller.UsbmuxdDeviceInfoNative>(ptr->device);
            var managed = new UsbmuxdEvent()
            {
                device = UsbmuxdDeviceInfoMarshaller.ConvertToManaged(device),
                @event = (UsbmuxdEventType)ptr->@event,
            };
            return managed;
        }

        public static void Free(nint unmanaged)
        {
            Marshal.FreeCoTaskMem(unmanaged);
        }
    }
}
#endif
