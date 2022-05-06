using System;
using System.IO;
using IOSLib.PropertyList;
using static IOSLib.Native.Usbmuxd;

namespace IOSLib
{
    public static class UsbMuxdService
    {
        public static PlistDocument ReadPairRecord(string udid)
        {
            if (usbmuxd_read_pair_record(udid, out var dataPtr, out uint recordSize)==0)
            {
                PlistDocument doc;
                unsafe
                {
                    using var ums = new UnmanagedMemoryStream((byte*)dataPtr.ToPointer(), recordSize);
                    doc = PlistDocument.Load(ums)!;
                }
                return doc;
            }
            throw new Exception();
        }
    }
}
