﻿using MobileDeviceSharp.Native;
using MobileDeviceSharp.NotificationProxy;
using MobileDeviceSharp.PropertyList;
using MobileDeviceSharp.PropertyList.Native;
using MobileDeviceSharp.Usbmuxd;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static MobileDeviceSharp.Native.Lockdown;

namespace MobileDeviceSharp
{
    /// <summary>
    /// Represente a session of the <see href="https://docs.libimobiledevice.org/libimobiledevice/latest/lockdown_8h.html">Lockdown</see> service.
    /// </summary>
    public sealed partial class LockdownSession : IOSHandleWrapperBase<LockdownClientHandle>
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
        /// Get the device Udid.
        /// </summary>
        public string DeviceUdid
        {
            get
            {
                lockdownd_get_device_udid(Handle, out var udid);
                return udid;
            }
        }

        /// <summary>
        /// Get a Lockdown domain with the specified <paramref name="domainName"/>.
        /// </summary>
        /// <param name="domainName">The name of the domain.</param>
        /// <returns>The requested <see cref="LockdownDomain"/>.</returns>
        public LockdownDomain GetDomain(string? domainName) => new(this, domainName);

        /// <summary>
        /// Get the default Lockdown domain.
        /// </summary>
        /// <returns>The requested <see cref="LockdownDomain"/>.</returns>
        public LockdownDomain GetDomain() => GetDomain(null);

        /// <summary>
        /// Try to get a lockdown domain with the specified <paramref name="domainName"/>.
        /// </summary>
        /// <param name="domainName">The name of the domain requested domain.</param>
        /// <param name="domain">The requested <see cref="LockdownDomain"/>.</param>
        /// <returns></returns>
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
        /// Start the Lockdown service with the specified <paramref name="serviceID"/>
        /// </summary>
        /// <param name="serviceID">The service identifier</param>
        /// <returns>The service descriptor handle</returns>
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
        /// <returns>The service descriptor handle.</returns>
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
        /// Start a pairing operation with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <param name="pairRecordHandle">The handle of the pair record.</param>
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
            using var np = new InsecureNotificationProxySession(_device);
            var requestPairTask = np.ObserveNotificationAsync("com.apple.mobile.lockdown.request_pair", cancellationToken);
            LockdownError? err = null;
            PairingState? lastPairingReport = null;
            while (true)
            {
                err = lockdownd_pair(Handle, pairRecordHandle);
                switch (err)
                {
                    case LockdownError.Success:
                        progress.Report(PairingState.Success);
                        _device.IsPaired = true;
                        return _device.IsPaired;
                    case LockdownError.UserDeniedPairing:
                        progress.Report(PairingState.UserDeniedPairing);
                        _device.IsPaired = false;
                        return _device.IsPaired;
                    case LockdownError.PasswordProtected:
                        if (lastPairingReport != PairingState.PasswordProtected)
                        {
                            progress.Report(PairingState.PasswordProtected);
                            lastPairingReport = PairingState.PasswordProtected;
                        }
                        break;
                    case LockdownError.PairingDialogResponsePending:
                        if (lastPairingReport != PairingState.PairingDialogResponsePending)
                        {
                            progress.Report(PairingState.PairingDialogResponsePending);
                            lastPairingReport = PairingState.PairingDialogResponsePending;
                        }
                        break;
                    default:
                        _device.IsPaired = false;
                        throw ((LockdownError)err).GetException();
                }
                await Task.WhenAny(Task.Delay(200,cancellationToken), requestPairTask).ConfigureAwait(false);
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
        /// Start a pairing operation.
        /// </summary>
        /// <returns>Return <see langword="true"/> if the user accept pairng else <see langword="false"/>.</returns>
        public Task<bool> PairAsync()
        {
            return PairAsync(LockdownPairRecordHandle.Zero, new Progress<PairingState>(), CancellationToken.None);
        }

        /// <summary>
        /// Start a pairing operation with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <param name="pairRecordHandle">The handle of the pairRecord</param>
        /// <returns>Return <see langword="true"/> if the user accept pairng else <see langword="false"/>.</returns>
        public Task<bool> PairAsync(LockdownPairRecordHandle pairRecordHandle)
        {
            return PairAsync(pairRecordHandle, new Progress<PairingState>(), CancellationToken.None);
        }

        /// <summary>
        /// Start a pairing operation with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <param name="pairRecordHandle">The handle of the pairRecord</param>
        /// <param name="progress">Used to report the progress</param>
        /// <returns>Return <see langword="true"/> if the user accept pairng else <see langword="false"/>.</returns>
        public Task<bool> PairAsync(LockdownPairRecordHandle pairRecordHandle, IProgress<PairingState> progress)
        {
            return PairAsync(pairRecordHandle, progress, CancellationToken.None);
        }

        /// <summary>
        /// Start a pairing operation with the specified <paramref name="pairRecordHandle"/>.
        /// </summary>
        /// <param name="pairRecordHandle">The handle of the pairRecord</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Return <see langword="true"/> if the user accept pairng else <see langword="false"/>.</returns>
        public Task<bool> PairAsync(LockdownPairRecordHandle pairRecordHandle, CancellationToken token)
        {
            return PairAsync(pairRecordHandle, new Progress<PairingState>(), token);
        }

        /// <summary>
        /// Start a pairing operation.
        /// </summary>
        /// <param name="cancellationToken">A cancelation token used to cancel stop the operation</param>
        /// <returns>Return <see langword="true"/> if the user accept pairng else <see langword="false"/>.</returns>
        public Task<bool> PairAsync(CancellationToken cancellationToken)
        {
            return PairAsync(LockdownPairRecordHandle.Zero, new Progress<PairingState>(), cancellationToken);
        }

        /// <summary>
        /// Start a pairing operation.
        /// </summary>
        /// <param name="progress">Used to report the progress</param>
        /// <returns>Return <see langword="true"/> if the user accept pairng else <see langword="false"/>.</returns>
        public Task<bool> PairAsync(IProgress<PairingState> progress)
        {
            return PairAsync(LockdownPairRecordHandle.Zero, progress, CancellationToken.None);
        }

        /// <summary>
        /// Start a pairing operation.
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
        /// Try to unpair the device.
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
