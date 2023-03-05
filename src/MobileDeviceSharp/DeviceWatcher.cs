using MobileDeviceSharp.Native;
using MobileDeviceSharp.Usbmuxd.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp
{
    /// <summary>
    /// A watcher used to detect device connection.
    /// </summary>
    public sealed class DeviceWatcher : IDisposable
    {
        private UsbmuxdSubscriptionContextHandle handle = UsbmuxdSubscriptionContextHandle.Zero;
        private readonly UsbmuxdEventCallBack _callBack;

        /// <summary>
        /// Raised when a device is added.
        /// </summary>
        public event EventHandler<DeviceEventArgs> DeviceAdded;

        /// <summary>
        /// Raised when a device is removed.
        /// </summary>
        public event EventHandler<DeviceEventArgs>? DeviceRemoved;

        /// <summary>
        /// Raised when a device is paired.
        /// </summary>
        public event EventHandler<DeviceEventArgs>? DevicePaired;

        private System.Threading.SynchronizationContext? _context;

        /// <summary>
        /// Initialise a new instance of <see cref="DeviceWatcher"/>.
        /// </summary>
        public DeviceWatcher() : this(IDeviceLookupOptions.All)
        {

        }

        /// <summary>
        /// /// Initialise a new instance of <see cref="DeviceWatcher"/>.
        /// </summary>
        /// <param name="connectionType">The specified connection lookup option.</param>
        public DeviceWatcher(IDeviceLookupOptions connectionType)
        {
            _callBack = new UsbmuxdEventCallBack(Callback);
            ConnectionType = connectionType;
        }

        /// <summary>
        /// Starts watching for device connections.
        /// </summary>
        public void Start()
        {
            if (!IsRunning)
            {
                _context = System.Threading.SynchronizationContext.Current ?? new System.Threading.SynchronizationContext();
                Usbmuxd.Native.Usbmuxd.usbmuxd_events_subscribe(out handle, _callBack, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the watcher is running.
        /// </summary>
        public bool IsRunning => !handle.IsInvalid;

        /// <summary>
        /// Gets the type of device connections to watch for.
        /// </summary>
        public IDeviceLookupOptions ConnectionType { get; }

        /// <summary>
        /// Stops watching for device connections.
        /// </summary>
        public void Stop()
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

        /// <inheritdoc/>
        public void Dispose() => Stop();
    }
}
