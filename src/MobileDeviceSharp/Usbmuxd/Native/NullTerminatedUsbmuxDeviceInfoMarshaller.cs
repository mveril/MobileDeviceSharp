#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using MobileDeviceSharp.Native;

namespace MobileDeviceSharp.Usbmuxd.Native
{
    [CustomMarshaller(typeof(UsbmuxdDeviceInfo[]), MarshalMode.Default, typeof(NullTerminatedUsbmuxDeviceInfoMarshaller))]
    internal static unsafe class NullTerminatedUsbmuxDeviceInfoMarshaller      
    {
        public static UsbmuxdDeviceInfo[] ConvertToManaged(IntPtr unmanaged)
        {
            var unmanagedArray = MarshalUtils.GetNullOrDefaultTerminatedSpan((UsbmuxdDeviceInfoMarshaller.UsbmuxdDeviceInfoNative**)unmanaged);
            var managedArray = new UsbmuxdDeviceInfo[unmanagedArray.Length];
            for (int i = 0; i < unmanagedArray.Length; i++)
            {
                managedArray[i] = UsbmuxdDeviceInfoMarshaller.ConvertToManaged(*(UsbmuxdDeviceInfoMarshaller.UsbmuxdDeviceInfoNative*)unmanagedArray[i]);
            }
            return managedArray;
        }

        public static void Free(IntPtr unmanaged)
        {
            var unmanagedArray = MarshalUtils.GetNullTerminatedSpan((IntPtr*)unmanaged);
            foreach (var item in unmanagedArray)
            {
                UsbmuxdDeviceInfoMarshaller.Free(*(UsbmuxdDeviceInfoMarshaller.UsbmuxdDeviceInfoNative*)item);
            }
            Marshal.FreeCoTaskMem(unmanaged);
        }
    }
}
#endif
