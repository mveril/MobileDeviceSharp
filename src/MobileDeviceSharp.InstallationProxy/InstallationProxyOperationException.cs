using System;
using System.Collections.Generic;
using System.Text;
using static MobileDeviceSharp.InstallationProxy.Native.InstallationProxy;
using MobileDeviceSharp.PropertyList.Native;

namespace MobileDeviceSharp.InstallationProxy
{
    public class InstallationProxyOperationException : Exception
    {
        public InstallationProxyOperationException(string errorName, string errorDesc, ulong errorCode) : base(errorDesc)
        {
            Name = errorName;
            Code = errorCode;
        }

        static internal bool TryFromStatusPlist(PlistHandle statusPlist, out InstallationProxyOperationException? exception)
        {
            instproxy_status_get_error(statusPlist, out var errorName, out var errorDesc, out var errorCode);
            var result = errorCode != 0;
            exception = (result ? null : new InstallationProxyOperationException(errorName, errorDesc, errorCode));
            return result;
        }

        public string Name { get; }
        public ulong Code { get; }
    }
}
