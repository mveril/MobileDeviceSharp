using IOSLib.Native;
using IOSLib.PropertyList;
using IOSLib.PropertyList.Native;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static IOSLib.Native.Lockdown;

namespace IOSLib
{
    /// <summary>
    /// Represente a session of the <see href="https://docs.libimobiledevice.org/libimobiledevice/latest/lockdown_8h.html">Lockdown</see> service.
    /// </summary>
    public class LockdownSession : IOSHandleWrapperBase<LockdownClientHandle>
    {
        private readonly IDevice device;

        /// <summary>
        /// Create lockdown session for the specified <paramref name="device"/>, the specified <paramref name="label"/> and with handshake or not.
        /// </summary>
        /// <param name="device">The target device.</param>
        /// <param name="label">Set a label for the lockdow0</param>
        /// <param name="WithHandShake">Create lockdown session with handshake or not.</param>
        public LockdownSession(IDevice device, string? label, bool WithHandShake) : base(GetHandle(device, label, WithHandShake))
        {
            this.device = device;
        }

        private static LockdownClientHandle GetHandle(IDevice device, string? label, bool withHandShake)
        {
            var handle = LockdownClientHandle.Zero;
            LockdownException? ex;
            if (withHandShake)
            {
                ex = lockdownd_client_new_with_handshake(device.Handle, out handle, label).GetException();
            }
            else
            {
                ex = lockdownd_client_new(device.Handle, out handle, label).GetException();
            }
            if (ex != null)
            {
                throw ex;
            }
            if (withHandShake)
            {
                device.IsPaired = true;
            }
            return handle;
        }

        /// <summary>
        /// Create lockdown session for the specified <paramref name="device"/> without handshake.
        /// </summary>
        /// <param name="device">The target device.</param>
        public LockdownSession(IDevice device) : this(device, null, false)
        {

        }

        /// <summary>
        /// Create lockdown session for the specified <paramref name="device"/> and with handshake or not.
        /// </summary>
        /// <param name="device">The target device.</param>
        /// <param name="WithHandShake">Create lockdown session with handshake or not.</param>
        public LockdownSession(IDevice device, bool WithHandShake) : this(device, null, WithHandShake)
        {

        }

        /// <summary>
        /// Query the type of the lockdown.
        /// </summary>
        public string Type
        {
            get
            {
                lockdownd_query_type(Handle, out var type);
                return type;
            }
        }

        /// <summary>
        /// Get or set the Lockdown label
        /// </summary>
        public string? Label
        {
            get
            {
                var privateLockdown = Marshal.PtrToStructure<lockdownd_client_private>(Handle.DangerousGetHandle());
                return (string)UTF8Marshaler.GetInstance().MarshalNativeToManaged(privateLockdown.label);
            }
            set
            {
                lockdownd_client_set_label(Handle, value);
            }
        }

        /// <summary>
        /// Get or set the target device name (equivalent of <see cref="IDevice.Name"/>
        /// </summary>
        public string DeviceName
        {
            get
            {
                lockdownd_get_device_name(Handle, out var name);
                return name;
            }
            set
            {
                using var PlistName = new PlistString(value);
                SetValue("DeviceName", PlistName);
            }
        }

        /// <summary>
        /// Get the device <see cref="Ulid"/>
        /// </summary>
        public Ulid DeviceUdid
        {
            get
            {
                lockdownd_get_device_udid(Handle, out var udid);
                return Ulid.Parse(udid);
            }
        }

        /// <summary>
        /// Get a value for the specified <paramref name="domain"/> and <paramref name="key"/>.
        /// </summary>
        /// <param name="domain">The target domain.</param>
        /// <param name="key">The target key.</param>
        /// <returns>A <see cref="PlistNode"/> contained the result value</returns>
        public PlistNode GetValue(string? domain,string key)
        {
            var ex = lockdownd_get_value(Handle, domain, key, out var plistHandle).GetException();
            if (ex != null)
            {
                throw ex;
            }
            return PlistNode.From(plistHandle);
        }

        /// <summary>
        /// Get a value for the default domain and <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The target key.</param>
        /// <returns>A <see cref="PlistNode"/> contained the result value.</returns>
        public PlistNode GetValue(string key)
        {
            return GetValue(null, key);
        }

        /// <summary>
        /// Try to Get a Lockdown value and return the <see cref="LockdownError"/>.
        /// </summary>
        /// <param name="domain">The target domain.</param>
        /// <param name="key">The target key.</param>
        /// <param name="node">The result <see cref="PlistNode"/></param>
        /// <returns>The lockdownError</returns>
        public LockdownError TryGetValue(string? domain, string key, out PlistNode? node)
        {
            var err = lockdownd_get_value(Handle, domain, key, out var plistHandle);
            if (err == LockdownError.Success)
            {
                node = PlistNode.From(plistHandle);
            }
            else
            {
                node = null;
            }
            return err;
        }

        /// <summary>
        /// Get a value for the default domain and <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The target key.</param>
        /// <param name="node">The result <see cref="PlistNode"/></param>
        /// <returns>The lockdownError</returns>
        public LockdownError TryGetValue(string key, out PlistNode node)
        {
            return TryGetValue(null, key, out node);
        }

        /// <summary>
        /// Get all the values for the specified <paramref name="domain"/>
        /// </summary>
        /// <param name="domain"></param>
        /// <returns>The <see cref="PlistNode"/> result</returns>
        public PlistNode GetValues(string? domain)
        {
            lockdownd_get_value(Handle, domain, null, out var plistHandle);
            return PlistNode.From(plistHandle);
        }

        /// <summary>
        /// Get all the values for the default domain
        /// </summary>
        /// <returns>The <see cref="PlistNode"/> result</returns>
        public PlistNode GetValues()
        {
            return GetValues(null);
        }

        public LockdownError TryGetValues(string? domain, out PlistNode node)
        {
            var err = lockdownd_get_value(Handle, domain, null, out var plistHandle);
            node = PlistNode.From(plistHandle);
            return err;
        }

        /// <summary>
        /// try to get all the values for the default domain  and return the <see cref="LockdownError"/>.
        /// </summary>
        /// <param name="node">The <see cref="PlistNode"/> result</param>
        /// <returns>The LockdownError</returns>
        public LockdownError TryGetValues(out PlistNode node)
        {
            return TryGetValues(null, out node);
        }

        /// <summary>
        /// set the value for the specified <paramref name="domain"/> and <paramref name="key"/>.
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="key"></param>
        /// <param name="node"></param>
        public void SetValue(string? domain, string key, PlistNode node)
        {
            lockdownd_set_value(Handle, domain, key, node.Handle);
        }

        /// <summary>
        /// Set the value for the default domain and <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="node"></param>
        public void SetValue(string key,PlistNode node)
        {
            SetValue(null, key, node);
        }

        /// <summary>
        /// Start the Lockdown service with the specified <paramref name="serviceID"/>
        /// </summary>
        /// <param name="serviceID">The service identifier</param>
        /// <returns></returns>
        public LockdownServiceDescriptorHandle StartService(string serviceID)
        {
            var ex = lockdownd_start_service(Handle, serviceID, out var serviceDescriptorHandle).GetException();
            if (ex != null)
            {
                throw ex;
            }
            return serviceDescriptorHandle;
        }

        /// <summary>
        /// Start the Lockdown service with the specified <paramref name="serviceID"/> and with an escrow bag or not
        /// </summary>
        /// <param name="serviceID"></param>
        /// <param name="withEscrowBag"></param>
        /// <returns>THe service descriptor handle</returns>
        public LockdownServiceDescriptorHandle StartService(string serviceID, bool withEscrowBag)
        {
            if (withEscrowBag)
            {
                var ex = lockdownd_start_service_with_escrow_bag(Handle, serviceID, out var serviceDescriptorHandle).GetException();
                if (ex != null)
                {
                    throw ex;
                }
                return serviceDescriptorHandle;
            }
            else
            {
                return StartService(serviceID);
            }
        }

        /// <summary>
        /// Start a pairing operatoion with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <param name="pairRecordHandle">The handle of the pairRecord</param>
        /// <param name="cancellationToken">A cancelation token used to cancel stop the operation</param>
        /// <returns>Return true if the operation succeed</returns>
        public Task<bool> PairAsync(LockdownPairRecordHandle pairRecordHandle,CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> tsk = new TaskCompletionSource<bool>();
            if (cancellationToken.IsCancellationRequested)
            {
                tsk.TrySetCanceled(cancellationToken);
            }
            cancellationToken.Register(() => tsk.TrySetCanceled(cancellationToken));
            var err = lockdownd_pair(Handle, pairRecordHandle);
            switch (err)
            {
                case LockdownError.Success:
                    device.IsPaired = true;
                    tsk.SetResult(true);
                    break;
                case LockdownError.UserDeniedPairing:
                    tsk.SetResult(false);
                    device.IsPaired = false;
                    break;
                case LockdownError.PairingDialogResponsePending:
                    var np = new InsecureNotificationProxySession(device);
                    np.ObserveNotification("com.apple.mobile.lockdown.request_pair");
                    np.NotificationProxyEvent += async (s, e) =>
                    {
                        tsk.SetResult(await PairAsync(pairRecordHandle, cancellationToken));
                        np.Dispose();
                    };
                    break;
                case LockdownError.InvalidHostId:
                    tsk.SetResult(false);
                    device.IsPaired = false;
                    break;
                default:
                    tsk.SetException(err.GetException());
                    break;
            }
            return tsk.Task;
        }

        /// <summary>
        /// Start a pairing operatoion.
        /// </summary>
        /// <returns>Return true if the operation succeed</returns>
        public Task<bool> PairAsync()
        {
            return PairAsync(LockdownPairRecordHandle.Zero,CancellationToken.None);
        }

        /// <summary>
        /// Start a pairing operatoion with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <param name="pairRecordHandle">The handle of the pairRecord.</param>
        /// <returns>Return true if the operation succeed</returns>
        public Task<bool> PairAsync(LockdownPairRecordHandle pairRecordHandle)
        {
            return PairAsync(pairRecordHandle, CancellationToken.None);
        }

        /// <summary>
        /// Start a pairing operatoion.
        /// </summary>
        /// <param name="cancellationToken">A cancelation token used to cancel stop the operation</param>
        /// <returns>Return true if the operation succeed</returns>
        public Task<bool> PairAsync(CancellationToken cancellationToken)
        {
            return PairAsync(LockdownPairRecordHandle.Zero, cancellationToken);
        }

        /// <summary>
        /// Try to unpair the device with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <param name="pairRecordHandle">The handle of the pairRecord.</param>
        /// <returns></returns>
        public bool Unpair(LockdownPairRecordHandle pairRecordHandle)
        {
            var b = !lockdownd_unpair(Handle, pairRecordHandle).IsError();
            device.IsPaired = !b;
            return b;
        }

        /// <summary>
        /// Try to unpair the device with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <returns></returns>
        public bool Unpair()
        {
            return Unpair(LockdownPairRecordHandle.Zero);
        }

        /// <summary>
        /// Put the device into the recovery mode
        /// </summary>
        public void EnterRecovery() 
        {
            lockdownd_enter_recovery(Handle);
        }
    }
}
