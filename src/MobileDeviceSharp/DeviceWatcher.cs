using MobileDeviceSharp.Native;
using MobileDeviceSharp.Usbmuxd.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp
{
    public sealed class DeviceWatcher : IDisposable
    {
        private UsbmuxdSubscriptionContextHandle handle = UsbmuxdSubscriptionContextHandle.Zero;
        private readonly UsbmuxdEventCallBack _callBack;
        public event EventHandler<DeviceEventArgs>? DeviceAdded, DeviceRemoved, DevicePaired;
        private System.Threading.SynchronizationContext? _context;
        public DeviceWatcher() : this(IDeviceLookupOptions.All)
        {

        }

        public DeviceWatcher(IDeviceLookupOptions connectionType)
        {
            _callBack = new UsbmuxdEventCallBack(Callback);
            ConnectionType = connectionType;
        }

        public void Start()
        {
            if (!IsRunning)
            {
                _context = System.Threading.SynchronizationContext.Current ?? new System.Threading.SynchronizationContext();
                Usbmuxd.Native.Usbmuxd.usbmuxd_events_subscribe(out handle, _callBack, IntPtr.Zero);
            }
        }

        public bool IsRunning => !handle.IsInvalid;

        public IDeviceLookupOptions ConnectionType { get; }

        private void Stop()
        {
            if (IsRunning)
            {
                handle.Close();
                handle = UsbmuxdSubscriptionContextHandle.Zero;
            }
        }


        private void Callback(ref UsbmuxdEvent @event, IntPtr userData)
        {
            if (ConnectionType.HasFlag(@event.device.conn_type))
            {
                _context?.Post((e) => OnEvent((UsbmuxdEvent)e), @event);
            }
        }

        private void OnEvent(UsbmuxdEvent @event)
        {
            switch (@event.@event)
            {
                case UsbmuxdEventType.DeviceAdd:
                    OnAdded(new DeviceEventArgs(@event.device));
                    break;
                case UsbmuxdEventType.DeviceRemove:
                    OnRemoved(new DeviceEventArgs(@event.device));
                    break;
                case UsbmuxdEventType.DevicePaired:
                    OnPaired(new DeviceEventArgs(@event.device));
                    break;
            }
        }

        private void OnAdded(DeviceEventArgs deviceEventArgs)
        {
            DeviceAdded?.Invoke(this, deviceEventArgs);
        }

        private void OnRemoved(DeviceEventArgs deviceEventArgs)
        {
            DeviceRemoved?.Invoke(this, deviceEventArgs);
        }

        private void OnPaired(DeviceEventArgs deviceEventArgs)
        {
            DevicePaired?.Invoke(this, deviceEventArgs);
        }

        public void Dispose() => Stop();
    }
}
