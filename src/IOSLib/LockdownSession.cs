using IOSLib.Native;
using IOSLib.NotificationProxy;
using IOSLib.PropertyList;
using IOSLib.PropertyList.Native;
using IOSLib.Usbmuxd;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static IOSLib.Native.Lockdown;

namespace IOSLib
{
    /// <summary>
    /// Represente a session of the <see href="https://docs.libimobiledevice.org/libimobiledevice/latest/lockdown_8h.html">Lockdown</see> service.
    /// </summary>
    public partial class LockdownSession : IOSHandleWrapperBase<LockdownClientHandle>
    {
        private readonly IDevice _device;

        /// <summary>
        /// Create lockdown session for the specified <paramref name="device"/>, the specified <paramref name="label"/> and with handshake or not.
        /// </summary>
        /// <param name="device">The target device.</param>
        /// <param name="label">Set a label for the lockdow0</param>
        /// <param name="WithHandShake">Create lockdown session with handshake or not.</param>
        public LockdownSession(IDevice device, string? label, bool WithHandShake) : base(GetHandle(device, label, WithHandShake))
        {
            _device = device;
        }

        private static LockdownClientHandle GetHandle(IDevice device, string? label, bool withHandShake)
        {
            LockdownClientHandle handle;
            LockdownError hresult;
            if (withHandShake)
            {
                hresult = lockdownd_client_new_with_handshake(device.Handle, out handle, label);
            }
            else
            {
                hresult = lockdownd_client_new(device.Handle, out handle, label);
            }
            if (hresult.IsError())
                throw hresult.GetException();
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
                GetDomain()["DeviceName"] = PlistName;
            }
        }

        /// <summary>
        /// Get the device <see cref="Ulid"/>
        /// </summary>
        public string DeviceUdid
        {
            get
            {
                lockdownd_get_device_udid(Handle, out var udid);
                return udid;
            }
        }

        public LockdownDomain GetDomain(string? domainName) => new(this, domainName);

        public LockdownDomain GetDomain() => GetDomain(null);

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        public bool TryGetDomain(string domainName, [MaybeNullWhen(false)] out LockdownDomain domain)
#else
        public bool TryGetDomain(string domainName, out LockdownDomain domain)
#endif
        {
            try
            {
                domain = GetDomain(domainName);
            }
            catch (Exception)
            {
                domain = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get a value for the specified <paramref name="domain"/> and <paramref name="key"/>.
        /// </summary>
        /// <param name="domain">The target domain.</param>
        /// <param name="key">The target key.</param>
        /// <returns>A <see cref="PlistNode"/> contained the result value</returns>
        [Obsolete("Use GetDomain(domain)[key];")]
        public PlistNode GetValue(string? domain, string key)
        {
            return GetDomain(domain)[key];
        }

        /// <summary>
        /// Get a value for the default domain and <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The target key.</param>
        /// <returns>A <see cref="PlistNode"/> contained the result value.</returns>
        [Obsolete("Use GetDomain()[key];")]
        public PlistNode GetValue(string key)
        {
            return GetDomain()[key];
        }

        /// <summary>
        /// Try to Get a Lockdown value and return the <see cref="LockdownError"/>.
        /// </summary>
        /// <param name="domain">The target domain.</param>
        /// <param name="key">The target key.</param>
        /// <param name="node">The result <see cref="PlistNode"/></param>
        /// <returns>The lockdownError</returns>
        [Obsolete("Use GetDomain(domain).TryGetValue(key, out node);")]
        public LockdownError TryGetValue(string? domain, string key, out PlistNode? node)
        {
            var hresult = lockdownd_get_value(Handle, domain, key, out var hNode);
            node = PlistNode.From(hNode);
            return hresult;
        }

        /// <summary>
        /// Get a value for the default domain and <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The target key.</param>
        /// <param name="node">The result <see cref="PlistNode"/></param>
        /// <returns>The lockdownError</returns>
        [Obsolete("Use GetDomain().TryGetValue(key, out node);")]
        public LockdownError TryGetValue(string key, out PlistNode? node)
        {
            return TryGetValue(null, key, out node);
        }

        /// <summary>
        /// Get all the values for the specified <paramref name="domain"/>
        /// </summary>
        /// <param name="domain"></param>
        /// <returns>The <see cref="PlistNode"/> result</returns>
        [Obsolete("Use GetDomain(domain).ToDictionary();")]
        public PlistNode GetValues(string? domain)
        {
            return GetDomain(domain).ToDictionary();
        }

        /// <summary>
        /// Get all the values for the default domain
        /// </summary>
        /// <returns>The <see cref="PlistNode"/> result</returns>
        [Obsolete("Use GetDomain().ToDictionary();")]
        public PlistNode GetValues()
        {
            return GetValues(null);
        }
        [Obsolete()]
        public LockdownError TryGetValues(string? domain, out PlistNode? node)
        {
            var err = lockdownd_get_value(Handle, domain, null, out var plistHandle);
            node = PlistNode.From(plistHandle);
            return err;
        }

        /// <summary>
        /// set the value for the specified <paramref name="domain"/> and <paramref name="key"/>.
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="key"></param>
        /// <param name="node"></param>
        [Obsolete("Use GetDomain(domain)[key] = node;")]
        public void SetValue(string? domain, string key, PlistNode node)
        {
            GetDomain(domain)[key] = node;
        }

        /// <summary>
        /// Set the value for the default domain and <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="node"></param>
        [Obsolete("Use GetDomain()[key] = node;;")]
        public void SetValue(string key, PlistNode node)
        {
            GetDomain()[key] = node;
        }

        /// <summary>
        /// Start the Lockdown service with the specified <paramref name="serviceID"/>
        /// </summary>
        /// <param name="serviceID">The service identifier</param>
        /// <returns></returns>
        public LockdownServiceDescriptorHandle StartService(string serviceID)
        {
            var hresult = lockdownd_start_service(Handle, serviceID, out var serviceDescriptorHandle);
            if (hresult.IsError())
                throw hresult.GetException();
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
                var hresult = lockdownd_start_service_with_escrow_bag(Handle, serviceID, out var serviceDescriptorHandle);
                if (hresult.IsError())
                {
                    throw hresult.GetException();
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
        /// <param name="progress">Used to report the progress</param>
        /// <param name="cancellationToken">A cancelation token used to cancel stop the operation</param>
        /// <returns>Return <see langword="true"/> if the user accept pairng else <see langword="false"/>.</returns>
        private async Task<bool> PairAsync(LockdownPairRecordHandle pairRecordHandle, IProgress<PairingState> progress, CancellationToken cancellationToken)
        {
            var result = await PairCoreAsync(pairRecordHandle, progress, cancellationToken).ConfigureAwait(false);
            if (result)
            {
                PerformHandshake(pairRecordHandle);
            }
            return result;
        }
        private async Task<bool> PairCoreAsync(LockdownPairRecordHandle pairRecordHandle, IProgress<PairingState> progress, CancellationToken cancellationToken)
        {
            var err = LockdownError.UnknownError;
            do
            {
                err = lockdownd_pair(Handle, pairRecordHandle);
                switch (err)
                {
                    case LockdownError.Success:
                        progress.Report(PairingState.Success);
                        break;
                    case LockdownError.UserDeniedPairing:
                        progress.Report(PairingState.UserDeniedPairing);
                        break;
                    case LockdownError.PasswordProtected:
                        progress.Report(PairingState.PasswordProtected);
                        do
                        {
                            await Task.Delay(200, cancellationToken);
                            cancellationToken.ThrowIfCancellationRequested();
                            err = lockdownd_pair(Handle, pairRecordHandle);
                        } while (err is LockdownError.PasswordProtected);
                        break;
                    case LockdownError.PairingDialogResponsePending:
                        progress.Report(PairingState.PairingDialogResponsePending);
                        using (var np = new InsecureNotificationProxySession(_device))
                        {
                            await np.ObserveNotificationAsync("com.apple.mobile.lockdown.request_pair", cancellationToken).ConfigureAwait(false);
                        }
                        cancellationToken.ThrowIfCancellationRequested();
                        break;
                    default:
                        throw err.GetException();
                        break;
                }
            } while (err is not (LockdownError.Success or LockdownError.UserDeniedPairing));
            if (err.IsError())
            {
                _device.IsPaired = false;
                if (err is LockdownError.UserDeniedPairing)
                {
                    return false;
                }
                else
                {
                    throw err.GetException();
                }
            }
            else
            {
                _device.IsPaired = true;
                return true;
            }
        }

        private void PerformHandshake(LockdownPairRecordHandle pairRecordHandle)
        {
            var hresult = LockdownError.Success;
            if (GetOSVersion() < new Version(7, 0) && GetDeviceClass() != DeviceClass.Watch)
            {
                hresult = lockdownd_validate_pair(Handle, pairRecordHandle);
                if (hresult.IsError())
                    throw hresult.GetException();
            }
            if(UsbmuxdService.TryReadPairRecord(DeviceUdid, out var doc))
            {
                try
                {
                    using var hostId = (PlistString)((PlistDictionary)doc.RootNode)["HostID"];
                    hresult = lockdownd_start_session(Handle, hostId.Value, out _, out _);
                    if (hresult.IsError())
                    {
                        throw hresult.GetException();
                    }
                }
                finally
                {
                    doc.Dispose();
                }
            }
        }

        internal Version GetOSVersion()
        {
            using PlistString plVersion = (PlistString)GetDomain()["ProductVersion"];
            return new Version(plVersion.Value);
        }

        /// <summary>
        /// Start a pairing operatoion with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <returns>Return <see langword="true"/> if the user accept pairng else <see langword="false"/>.</returns>
        public Task<bool> PairAsync()
        {
            return PairAsync(LockdownPairRecordHandle.Zero, new Progress<PairingState>(), CancellationToken.None);
        }

        /// <summary>
        /// Start a pairing operatoion with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <param name="pairRecordHandle">The handle of the pairRecord</param>
        /// <returns>Return <see langword="true"/> if the user accept pairng else <see langword="false"/>.</returns>
        public Task<bool> PairAsync(LockdownPairRecordHandle pairRecordHandle)
        {
            return PairAsync(pairRecordHandle, new Progress<PairingState>(), CancellationToken.None);
        }

        /// <summary>
        /// Start a pairing operatoion with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <param name="pairRecordHandle">The handle of the pairRecord</param>
        /// <param name="progress">Used to report the progress</param>
        /// <returns>Return <see langword="true"/> if the user accept pairng else <see langword="false"/>.</returns>
        public Task<bool> PairAsync(LockdownPairRecordHandle pairRecordHandle, IProgress<PairingState> progress)
        {
            return PairAsync(pairRecordHandle, progress, CancellationToken.None);
        }

        /// <summary>
        /// Start a pairing operatoion with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <param name="pairRecordHandle">The handle of the pairRecord</param>
        /// <returns>Return <see langword="true"/> if the user accept pairng else <see langword="false"/>.</returns>
        public Task<bool> PairAsync(LockdownPairRecordHandle pairRecordHandle, CancellationToken token)
        {
            return PairAsync(pairRecordHandle, new Progress<PairingState>(), token);
        }

        /// <summary>
        /// Start a pairing operatoion with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <param name="cancellationToken">A cancelation token used to cancel stop the operation</param>
        /// <returns>Return <see langword="true"/> if the user accept pairng else <see langword="false"/>.</returns>
        public Task<bool> PairAsync(CancellationToken cancellationToken)
        {
            return PairAsync(LockdownPairRecordHandle.Zero, new Progress<PairingState>(), cancellationToken);
        }

        /// <summary>
        /// Start a pairing operatoion with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <param name="progress">Used to report the progress</param>
        /// <returns>Return <see langword="true"/> if the user accept pairng else <see langword="false"/>.</returns>
        public Task<bool> PairAsync(IProgress<PairingState> progress)
        {
            return PairAsync(LockdownPairRecordHandle.Zero, progress, CancellationToken.None);
        }

        /// <summary>
        /// Start a pairing operatoion with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <param name="progress">Used to report the progress</param>
        /// <param name="cancellationToken">A cancelation token used to cancel stop the operation</param>
        /// <returns>Return <see langword="true"/> if the user accept pairng else <see langword="false"/>.</returns>
        public Task<bool> PairAsync(IProgress<PairingState> progress, CancellationToken cancellationToken)
        {
            return PairAsync(LockdownPairRecordHandle.Zero, progress, cancellationToken);
        }

        /// <summary>
        /// Try to unpair the device with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <param name="pairRecordHandle">The handle of the pairRecord.</param>
        /// <returns></returns>
        public bool Unpair(LockdownPairRecordHandle pairRecordHandle)
        {
            var b = !lockdownd_unpair(Handle, pairRecordHandle).IsError();
            _device.IsPaired = !b;
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
        /// Try to put the device into recovery mode
        /// </summary>
        /// <returns>The error code</returns>
        public LockdownError TryEnterRecovery()
        {
            return lockdownd_enter_recovery(Handle);
        }


        internal string GetRawDeviceClass()
        {
            using PlistString deviceClassNode = (PlistString)GetDomain()["DeviceClass"];
            return deviceClassNode.Value;
        }

        internal DeviceClass GetDeviceClass()
        {
            if (Enum.TryParse<DeviceClass>(GetRawDeviceClass(), out var dClass))
            {
                return dClass;
            }
            else
            {
                return DeviceClass.Unknow;
            }
        }
    }
}
