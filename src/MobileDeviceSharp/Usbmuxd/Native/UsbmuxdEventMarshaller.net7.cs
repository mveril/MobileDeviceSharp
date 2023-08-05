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
        internal struct UsbmuxdEventNative
        {
            public int @event;

            public UsbmuxdDeviceInfoMarshaller.UsbmuxdDeviceInfoNative device;
        }

        public static UsbmuxdEventNative ConvertToUnmanaged(UsbmuxdEvent managed)
        {
            var unmanaged = new UsbmuxdEventNative()
            {
                @event = (int)managed.@event,
                device = UsbmuxdDeviceInfoMarshaller.ConvertToUnmanaged(managed.device),
            };
            return unmanaged;
        }

        public static UsbmuxdEvent ConvertToManaged(UsbmuxdEventNative unmanaged)
        {
            var managed = new UsbmuxdEvent()
            {
                device = UsbmuxdDeviceInfoMarshaller.ConvertToManaged(unmanaged.device),
                @event = (UsbmuxdEventType)unmanaged.@event,
            };
            return managed;
        }

        public static void Free(UsbmuxdEventNative unmanaged)
        {

        }
    }
}
#endif
