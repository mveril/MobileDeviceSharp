using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using IOSLib;
using IOSLib.Native;
using PlistSharp;

namespace IOSLib
{

    public class Lockdown : IDisposable
    {
        public LockdownClientHandle Handle { get; } = LockdownClientHandle.Zero;

        public Lockdown(IDevice device,string? label, bool WithHandShake)
        {
            var handle = LockdownClientHandle.Zero;
            LockdownException? ex;
            if (WithHandShake)
            {
                ex = Native.Lockdown.lockdownd_client_new_with_handshake(device.Handle, out handle, label).GetException();
            }
            else
            {
                ex = Native.Lockdown.lockdownd_client_new(device.Handle, out handle, label).GetException();
            }
            if (ex != null)
            {
                throw ex;
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
                Native.Lockdown.lockdownd_query_type(Handle, out var type);
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
                Native.Lockdown.lockdownd_client_set_label(Handle, value);
            }
        }

        public string DeviceName
        {
            get
            {
                Native.Lockdown.lockdownd_get_device_name(Handle, out var name);
                return name;
            }
        }

        public string DeviceUdid
        {
            get
            {
                Native.Lockdown.lockdownd_get_device_udid(Handle, out var udid);
                return udid;
            }
        }

        public PlistNode GetValue(string? domain,string key)
        {
            Native.Lockdown.lockdownd_get_value(Handle, domain, key, out var plistNode);
            return plistNode;
        }

        public PlistNode GetValue(string value)
        {
            return GetValue(null, value);
        }

        public LockdownError TryGetValue(string? domain, string key, out PlistNode node)
        {
            var err = Native.Lockdown.lockdownd_get_value(Handle, domain, key, out node);
            if (err == LockdownError.GetProhibited)
            {
                Pair();
                err = Native.Lockdown.lockdownd_get_value(Handle, domain, key, out node);
            }
            return err;
        }

        public LockdownError TryGetValue(string value, out PlistNode node)
        {
            return TryGetValue(null, value, out node);
        }

        public PlistNode GetValues(string? domain)
        {
            Native.Lockdown.lockdownd_get_value(Handle, domain, null, out var plistNode);
            return plistNode;
        }

        public PlistNode GetValues()
        {
            return GetValues(null);
        }

        public LockdownError TryGetValues(string? domain, out PlistNode node)
        {
            var err = Native.Lockdown.lockdownd_get_value(Handle, domain, null, out node);
            if (err == LockdownError.GetProhibited)
            {
                Pair();
                err = Native.Lockdown.lockdownd_get_value(Handle, domain, null, out node);
            }
            return err;
        }

        public LockdownError TryGetValues(out PlistNode node)
        {
            return TryGetValues(null, out node);
        }

        public void SetValue(string? domain, string key, PlistNode node)
        {
            Native.Lockdown.lockdownd_set_value(Handle, domain, key, node);
        }

        public void SetValue(string value,PlistNode node)
        {
            SetValue(null, value, node);
        }

        public LockdownServiceDescriptorHandle StartService(string identifier)
        {
            var ex = Native.Lockdown.lockdownd_start_service(Handle, identifier, out var serviceDescriptorHandle).GetException();
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
                var ex = Native.Lockdown.lockdownd_start_service_with_escrow_bag(Handle, identifier, out var serviceDescriptorHandle).GetException();
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

        public bool Pair(Native.LockdownPairRecordHandle? pairRecordHandle)
        {
            return !Native.Lockdown.lockdownd_pair(Handle, pairRecordHandle).IsError();
        }


        public bool Pair()
        {
            return Pair(null);
        }

        public bool Unpair(Native.LockdownPairRecordHandle? pairRecordHandle)
        {
            return !Native.Lockdown.lockdownd_unpair(Handle, pairRecordHandle).IsError();
        }

        public bool Unpair()
        {
            return Unpair(null);
        }

        public bool ValidatePair()
        {
           var code = Native.Lockdown.lockdownd_validate_pair(Handle, null);
            return code switch
            {
                LockdownError.Success => true,
                LockdownError.InvalidHostId => false,
                _ => throw code.GetException(),
            };
        }

        void EnterRecovery() 
        {
            Native.Lockdown.lockdownd_enter_recovery(Handle);
        }

        public void Dispose()
        {
            ((IDisposable)Handle).Dispose();
        }
    }
}
