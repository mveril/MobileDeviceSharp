using System;
using System.IO;
using MobileDeviceSharp.PropertyList;
using System.Runtime.InteropServices;
using static MobileDeviceSharp.Usbmuxd.Native.Usbmuxd;
using MobileDeviceSharp.Usbmuxd.Native;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace MobileDeviceSharp.Usbmuxd
{
    public static class UsbmuxdService
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
        public static bool TryReadPairRecord(string udid, [MaybeNullWhen(false)] out PlistDocument pairRecordPlist)
#else
        public static bool TryReadPairRecord(string udid, out PlistDocument pairRecordPlist)
#endif
        {
            var success = usbmuxd_read_pair_record(udid, out IntPtr dataPtr, out uint recordSize) == 0;
            unsafe
            {
                if (success)
                {

                    using var ums = new UnmanagedMemoryStream((byte*)dataPtr, recordSize);
                    pairRecordPlist = PlistDocument.Load(ums)!;
                }
                else
                {
                    pairRecordPlist = null;
                }
                Marshal.FreeHGlobal(dataPtr);
                return success;
            }
        }

        public static bool TrySavePairRecord(string udid, PlistDocument pairRecordPlist)
        {
            // I use directly native methods because the length is uint
            IntPtr plistbin = IntPtr.Zero;
            bool success;
            try
            {
                PropertyList.Native.Plist.plist_to_bin(pairRecordPlist.RootNode.Handle, out plistbin, out var length);
                success = usbmuxd_save_pair_record(udid, plistbin, length) == 0;
            }
            finally
            {
                if (plistbin != IntPtr.Zero)
                {
                    PropertyList.Native.Plist.plist_to_bin_free(plistbin);
                }
            }
            return success;
        }

        public static bool TryDeletePairRecord(string udid)
        {
            return usbmuxd_delete_pair_record(udid) == 0;
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
        public static bool TryGetDeviceInfo(string udid, IDeviceLookupOptions connectionOption, [MaybeNullWhen(false)] out UsbmuxdDeviceInfo device)
#else
        public static bool TryGetDeviceInfo(string udid, IDeviceLookupOptions connectionOption, out UsbmuxdDeviceInfo device)
#endif
        {
            return usbmuxd_get_device(udid, out device, connectionOption) == 0;
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
        public static bool TryGetDeviceInfo(string udid, [MaybeNullWhen(false)] out UsbmuxdDeviceInfo device)
#else
        public static bool TryGetDeviceInfo(string udid, out UsbmuxdDeviceInfo device)
#endif
        {
            return usbmuxd_get_device_by_udid(udid, out device) == 0;
        }

        public static UsbmuxdSocketType SocketType
        {
            get
            {
                usbmuxd_get_socket_type(out var socketType);
                return socketType;
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static System.Net.EndPoint SocketEndPoint
        {
            get
            {
                switch (SocketType)
                {
                    case UsbmuxdSocketType.UNIX:
                        return new System.Net.Sockets.UnixDomainSocketEndPoint("/var/run/usbmuxd");
                    case UsbmuxdSocketType.TCP:
                        usbmuxd_get_tcp_endpoint(out var host, out var port);
                        return new System.Net.IPEndPoint(System.Net.IPAddress.Parse(host), port);
                    default:
                        throw new NotSupportedException();
                }
            }
        }
#endif
    }
}
