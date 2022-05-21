using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib.Native
{
    public delegate TError StartServiceCallback<THandle, TError>(IDeviceHandle device, out THandle client, string? label) where TError : Enum where THandle : IOSHandle;
}
