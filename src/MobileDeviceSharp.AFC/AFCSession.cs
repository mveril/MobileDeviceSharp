using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.AFC
{
    /// <summary>
    /// Represent a standard Apple file conduit session used to have access to the <see href="https://www.theiphonewiki.com/wiki//private/var/mobile/Media">Media</see> directory
    /// </summary>
    public sealed partial class AFCSession
    {
        /// <inheritdoc/>
        public override string RootPath => "/private/var/mobile/Media";

    }
}
