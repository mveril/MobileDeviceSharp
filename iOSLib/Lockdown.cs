using IOSLib.Native;
using PlistSharp;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static IOSLib.Native.Lockdown;

namespace IOSLib
{

    public class Lockdown : IDisposable
    {
        private readonly IDevice device;

        public LockdownClientHandle Handle { get; } = LockdownClientHandle.Zero;

        public Lockdown(IDevice device,string? label, bool WithHandShake)
        {
            var handle = LockdownClientHandle.Zero;
            this.device = device;
            LockdownException? ex;
            if (WithHandShake)
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
            if (WithHandShake)
            {
                device.IsPaired = true;
            }
            Handle = handle;
        }

        public Lockdown(IDevice device) : this(device, null, false)
        {

        }

        public Lockdown(IDevice device, bool WithHandShake) : this(device, null, WithHandShake)
        {

        }


        public string Type
        {
            get
            {
                lockdownd_query_type(Handle, out var type);
                return type;
            }
        }

        public string? Label
        {
            get
            {
                var privateLockdown = Marshal.PtrToStructure<lockdownd_client_private>(Handle.DangerousGetHandle());
                return privateLockdown.label;
            }
            set
            {
                lockdownd_client_set_label(Handle, value);
            }
        }

        public string DeviceName
        {
            get
            {
                lockdownd_get_device_name(Handle, out var name);
                return name;
            }
        }

        public Ulid DeviceUdid
        {
            get
            {
                lockdownd_get_device_udid(Handle, out var udid);
                return Ulid.Parse(udid);
            }
        }

        public PlistNode GetValue(string? domain,string key)
        {
            var ex = lockdownd_get_value(Handle, domain, key, out var nodet).GetException();
            if (ex != null)
            {
                throw ex;
            }
            return PlistNode.FromPlist(nodet);
        }

        public PlistNode GetValue(string value)
        {
            return GetValue(null, value);
        }

        public LockdownError TryGetValue(string? domain, string key, out PlistNode? node)
        {
            var err = lockdownd_get_value(Handle, domain, key, out var nodet);
            if (err == LockdownError.Success)
            {
                node = PlistNode.FromPlist(nodet);
            }
            else
            {
                node = null;
            }
            return err;
        }

        public LockdownError TryGetValue(string value, out PlistNode node)
        {
            return TryGetValue(null, value, out node);
        }

        public PlistNode GetValues(string? domain,bool Elevate = true)
        {
            lockdownd_get_value(Handle, domain, null, out var nodet);
            return PlistNode.FromPlist(nodet);
        }

        public PlistNode GetValues()
        {
            return GetValues(null);
        }

        public LockdownError TryGetValues(string? domain, out PlistNode node)
        {
            var err = lockdownd_get_value(Handle, domain, null, out var nodet);
            node = PlistNode.FromPlist(nodet);
            return err;
        }

        public LockdownError TryGetValues(out PlistNode node)
        {
            return TryGetValues(null, out node);
        }

        public void SetValue(string? domain, string key, PlistNode node)
        {
            lockdownd_set_value(Handle, domain, key, node);
        }

        public void SetValue(string value,PlistNode node)
        {
            SetValue(null, value, node);
        }

        public LockdownServiceDescriptorHandle StartService(string identifier)
        {
            var ex = lockdownd_start_service(Handle, identifier, out var serviceDescriptorHandle).GetException();
            if (ex != null)
            {
                throw ex;
            }
            return serviceDescriptorHandle;
        }

        public LockdownServiceDescriptorHandle StartService(string identifier,bool sendEscrowBag)
        {
            if (sendEscrowBag)
            {
                var ex = lockdownd_start_service_with_escrow_bag(Handle, identifier, out var serviceDescriptorHandle).GetException();
                if (ex != null)
                {
                    throw ex;
                }
                return serviceDescriptorHandle;
            }
            else
            {
                return StartService(identifier);
            }
        }





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
                    tsk.SetResult(true);
                    device.IsPaired = true;
                    break;
                case LockdownError.UserDeniedPairing:
                    tsk.SetResult(false);
                    device.IsPaired = false;
                    break;
                case LockdownError.PairingDialogResponsePending:
                    var np = new InsecureNotificationProxy(device);
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

        public Task<bool> PairAsync()
        {
            return PairAsync(LockdownPairRecordHandle.Zero,CancellationToken.None);
        }

        public Task<bool> PairAsync(LockdownPairRecordHandle pairRecordHandle)
        {
            return PairAsync(pairRecordHandle, CancellationToken.None);
        }

        public Task<bool> PairAsync(CancellationToken cancellationToken)
        {
            return PairAsync(LockdownPairRecordHandle.Zero, cancellationToken);
        }

        public bool Unpair(LockdownPairRecordHandle pairRecordHandle)
        {
            var b = !lockdownd_unpair(Handle, pairRecordHandle).IsError();
            device.IsPaired = !b;
            return b;
        }

        public bool Unpair()
        {
            return Unpair(LockdownPairRecordHandle.Zero);
        }

        void EnterRecovery() 
        {
            lockdownd_enter_recovery(Handle);
        }

        public void Dispose()
        {
            ((IDisposable)Handle).Dispose();
        }
    }
}
