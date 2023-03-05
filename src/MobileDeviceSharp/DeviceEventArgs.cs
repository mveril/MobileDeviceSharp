using MobileDeviceSharp.Usbmuxd.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp
{
    /// <summary>
    /// Represent an event associated to a connection, deconnection, or pairing of a Deivce.
    /// </summary>
    public sealed class DeviceEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a new instance of <see cref="DeviceEventArgs"/> from
        /// </summary>
        /// <param name="deviceInfo"></param>
        public DeviceEventArgs(UsbmuxdDeviceInfo deviceInfo)
        {
            Udid = deviceInfo.udid;
            ProductID = deviceInfo.product_id;
            ConnectionType = deviceInfo.conn_type;
        }

        /// <summary>
        /// Try get the device who rase this event.
        /// </summary>
        /// <param name="device">The resulted device (this value is null if result is <see langword="false"/></param>
        /// <returns><see langword="true"/> on succes else <see langword="false"/></returns>
        public bool TryGetDevice(out IDevice device)
        {
            return IDevice.TryGetDevice(Udid, ConnectionType, out device);
        }

        /// <summary>
        /// The device udid.
        /// </summary>
        public string Udid { get; }

        /// <summary>
        /// The device Product id.
        /// </summary>
        public uint ProductID { get; }
        /// <summary>
        /// The device connection type.
        /// </summary>
        public IDeviceLookupOptions ConnectionType { get; }
    }
}
