using IOSLib.Native;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib
{
    /// <summary>
    /// Represent a base class for a libmobiledevice service.
    /// </summary>
    /// <typeparam name="THandle">The type of <see cref="IOSHandle"/>.</typeparam>
    /// <typeparam name="TError">The type of the enum  representing the hresult of the service methods.</typeparam>
    public abstract class ServiceSessionBase<THandle, TError> : IOSHandleWrapperBase<THandle> where THandle : IOSHandle,new() where TError : Enum
    {
        protected ServiceSessionBase(IDevice device ,string serviceID, bool withEscrowBag, ClientNewCallback<THandle,TError> ClientNew) : base()
        {
            var init = ClientNew;
            Device = device;
            using var ld = new LockdownSession(device);
            var descriptor = ld.StartService(serviceID, withEscrowBag);
            var error = init(device.Handle, descriptor, out THandle handle);
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
            var error = init(device.Handle, out THandle handle, null);
            var ex = ExceptionUtils.GetException(error);
            if (ex != null)
            {
                throw ex;
            }
            Handle = handle;
        }

        /// <summary>
        /// Get the undeling device.
        /// </summary>
        public IDevice Device { get; }
    }
}
