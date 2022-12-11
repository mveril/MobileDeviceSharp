using System;

namespace MobileDeviceSharp.Native
{
    public delegate TChildError ClientNewFromParentServiceCallback<TParentHandle, TChildHandle, TChildError>(TParentHandle client, out TChildHandle childClient) where TChildError : Enum where TParentHandle : IOSHandle where TChildHandle : IOSHandle;
}
