using System;
using System.Collections.Generic;
using System.Text;
using static MobileDeviceSharp.InstallationProxy.Native.InstallationProxy;
using MobileDeviceSharp.PropertyList.Native;

namespace MobileDeviceSharp.InstallationProxy
{
    /// <summary>
    /// Represent an exception occuring during an installation proxy operation.
    /// </summary>
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

        /// <summary>
        /// Get the error name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Get the error code.
        /// </summary>
        public ulong Code { get; }
    }
}
