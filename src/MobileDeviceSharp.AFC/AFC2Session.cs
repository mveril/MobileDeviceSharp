using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.AFC
{
    /// <summary>
    /// Represent an Apple File Conduit 2 session used to get access to the whole filesystem of a jailbroken apple device.
    /// </summary>
    public sealed class AFC2Session : AFCSessionBase
    {
        private const string AFC2_SERVICE_ID = "com.apple.afc2";

        /// <summary>
        /// Create an <see cref="AFC2Session"/>
        /// </summary>
        /// <param name="device"></param>
        public AFC2Session(IDevice device) : base(device, AFC2_SERVICE_ID) { }

        /// <inheritdoc/>
        public override string RootPath => "/";
    }
}
