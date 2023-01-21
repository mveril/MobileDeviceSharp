using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.InstallationProxy
{
    /// <summary>
    /// Represent the type of archiving.
    /// </summary>
    [Flags]
    public enum ArchiveType
    {
        /// <summary>
        /// Achive only the application.
        /// </summary>
        Application,

        /// <summary>
        /// Achive only document.
        /// </summary>
        Documents,

        /// <summary>
        /// Archive all.
        /// </summary>
        All = Application | Documents
    }
}
