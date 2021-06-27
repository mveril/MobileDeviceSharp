using IOSLib.Native;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib
{
    public abstract class ServiceSessionBase<T> : IOSHandleWrapperBase<T> where T : IOSHandle,new()
    {
        protected ServiceSessionBase(IDevice device ,string? serviceID) : base()
        {
            Device = device;
            if (serviceID == null)
            {
                Handle = Init();
            }
            else
            {
                using var ld = new LockdownSession(device);
                var descriptor = ld.StartService(serviceID);
                Handle = Init(descriptor);
            }
        }

        protected ServiceSessionBase(IDevice device) : this(device, null)
        {

        }

        protected abstract T Init(LockdownServiceDescriptorHandle Descriptor);

        protected abstract T Init();

        public IDevice Device { get; }
    }
}
