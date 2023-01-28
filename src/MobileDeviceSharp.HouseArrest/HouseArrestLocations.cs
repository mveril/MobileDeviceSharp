using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.HouseArrest
{
    /// <summary>
    /// Represent the autorized location foran <see cref="AFCHouseArrestSession"/> (the whole <see cref="Container"/> or only the <see cref="Documents"/>.
    /// <para>
    ///    See the <see href="https://developer.apple.com/library/archive/documentation/FileManagement/Conceptual/FileSystemProgrammingGuide/FileSystemOverview/FileSystemOverview.html#//apple_ref/doc/uid/TP40010672-CH2-SW12">About the iOS File System</see> section on Apple Developper.
    /// </para>
    /// </summary>
    public enum HouseArrestLocation
    {
        /// <summary>
        /// Represent access to the whole container.
        /// </summary>
        Container,
        /// <summary>
        /// Represent access the /Document folder.
        /// </summary>
        Documents,
    }
}
