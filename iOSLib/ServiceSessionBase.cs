﻿using IOSLib.Native;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib
{
    public abstract class ServiceSessionBase<THandle, TError> : IOSHandleWrapperBase<THandle> where THandle : IOSHandle,new() where TError : Enum
    {
        protected ServiceSessionBase(IDevice device ,string serviceID, bool withEscrowBag, ClientNewCallback<THandle,TError> ClientNew) : base()
        {
            var init = ClientNew;
            Device = device;
            using var ld = new LockdownSession(device);
            var descriptor = ld.StartService(serviceID, withEscrowBag);
            THandle handle;
            var error = init(device.Handle, descriptor, out handle);
            var ex = ExceptionUtils.GetException(error);
            if (ex != null)
            {
                throw ex;
            }
            Handle = handle;
        }

        protected ServiceSessionBase(IDevice device, StartServiceCallback<THandle, TError> startService) : base()
        {
            var init = startService;
            Device = device;
            THandle handle;
            var error = init(device.Handle, out handle, null);
            var ex = ExceptionUtils.GetException(error);
            if (ex != null)
            {
                throw ex;
            }
            Handle = handle;
        }

        public IDevice Device { get; }
    }
}
