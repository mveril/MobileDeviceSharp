using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.PropertyList;

namespace MobileDeviceSharp.InstallationProxy
{
    /// <summary>
    /// Represent options for an installation proxy operation.
    /// </summary>
    public abstract class InstallationProxyOperationOptions
    {
        /// <summary>
        /// Get a <see cref="PlistDictionary"/> representing the current instance.
        /// </summary>
        /// <returns></returns>
        public abstract PlistDictionary? ToDictionary();
    }
}
