using IOSLib.Usbmuxd.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib
{
    public class DeviceEventArgs : EventArgs
    {
        public DeviceEventArgs(UsbmuxdDeviceInfo deviceInfo)
        {
            Udid = deviceInfo.udid;
            ProductID = deviceInfo.product_id;
            ConnectionType = deviceInfo.conn_type;
        }
        public bool TryGetDevice(out IOSLib.IDevice device)
        {
            device= new IDevice(Udid, ConnectionType);
            return device!=null;
        }

        public string Udid { get; }
        public uint ProductID { get; }
        public UsbmuxConnectionType ConnectionType { get; }
    }
}
