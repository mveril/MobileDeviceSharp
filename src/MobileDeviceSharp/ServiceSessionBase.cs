﻿using MobileDeviceSharp.Native;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp
{
    /// <summary>
    /// Represent a base class for a libmobiledevice service.
    /// </summary>
    /// <typeparam name="THandle">The type of <see cref="IOSHandle"/>.</typeparam>
    /// <typeparam name="TError">The type of the enum  representing the hresult of the service methods.</typeparam>
    public abstract class ServiceSessionBase<THandle, TError> : IOSHandleWrapperBase<THandle> where THandle : IOSHandle,new() where TError : Enum
    {
        /// <summary>
        /// Initialize a service session.
        /// </summary>
        /// <param name="device">The targeted device.</param>
        /// <param name="serviceID">The service identifier.</param>
        /// <param name="withEscrowBag">An escrow bag</param>
        /// <param name="ClientNew">The <see cref="ClientNewCallback{THandle, TError}"/>used to create the service.</param>
        protected ServiceSessionBase(IDevice device ,string serviceID, bool withEscrowBag, ClientNewCallback<THandle,TError> ClientNew) : base()
        {
            var init = ClientNew;
            Device = device;
            using var ld = new LockdownSession(device);
            var descriptor = ld.StartService(serviceID, withEscrowBag);
            var error = init(device.Handle, descriptor, out THandle handle);
            var ex = ExceptionUtils.GetException(error);
            if (ex is not null)
            {
                throw ex;
            }
            Handle = handle;
        }

        /// <summary>
        /// Initialize a service session.
        /// </summary>
        /// <param name="device">The targeted device.</param>
        /// <param name="startService">The <see cref="StartServiceCallback{THandle, TError}"/> used to create the service.</param>
        protected ServiceSessionBase(IDevice device, StartServiceCallback<THandle, TError> startService) : base()
        {
            var init = startService;
            Device = device;
            var error = init(device.Handle, out THandle handle, null);
            var ex = ExceptionUtils.GetException(error);
            if (ex is not null)
            {
                throw ex;
            }
            Handle = handle;
        }

        protected ServiceSessionBase(IDevice device, THandle handle) : base(handle)
        {
            Device = device;
        }

        /// <summary>
        /// Get the undeling device.
        /// </summary>
        public IDevice Device { get; }
    }
}
