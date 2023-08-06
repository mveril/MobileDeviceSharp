using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MobileDeviceSharp.PropertyList.Native
{
    /// <summary>
    /// A custom type of <see cref="PlistHandle"/> that not release underlying handle (using <see cref="PlistHandle.ReleaseHandle"/> when the SafeHandle is garbage collected (The native code should release it.
    /// </summary>
    public sealed class PlistNotOwnedHandle : PlistHandle
    {
        /// <inheritdoc/>
        public PlistNotOwnedHandle() : base(false)
        {
            Debugger.Break();
        }
    }
}
