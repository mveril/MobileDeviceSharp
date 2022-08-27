using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.Native
{
    public delegate TError ClientNewCallback<THandle,TError>(IDeviceHandle device, LockdownServiceDescriptorHandle lockdownServiceDescriptor, out THandle client) where TError : Enum where THandle : IOSHandle;
}
