using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.InstallationProxy
{
    /// <summary>
    /// Represent the type of an <see cref="Application"/>
    /// </summary>
    [Flags]
    public enum ApplicationType
    {
        /// <summary>
        /// Hidden app.
        /// </summary>
        Hidden = 1,

        /// <summary>
        /// System app.
        /// </summary>
        System = 2,

        /// <summary>
        /// Represent a user app (an app downloaded from the App Store).
        /// </summary>
        User = 4,

        /// <summary>
        /// Any kind of app.
        /// </summary>
        Any = Hidden | User | System,
    }
}
