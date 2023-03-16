using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.PropertyList;

namespace MobileDeviceSharp.SpringBoardServices
{
    /// <summary>
    /// Represents a SpringBoard disposition object and its associated version.
    /// </summary>
    public sealed class SpringBoardDissposition : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpringBoardDissposition"/> class with the specified <paramref name="disposition"/> and <paramref name="version"/>.
        /// </summary>
        /// <param name="disposition">The <see cref="PlistArray"/> object that represents the SpringBoard disposition.</param>
        /// <param name="version">The version of the SpringBoard disposition object.</param>
        internal SpringBoardDissposition(PlistArray disposition, string version)
        {
            Disposition = disposition;
            Version = version;
        }

        /// <summary>
        /// Gets the <see cref="PlistArray"/> object that represents the SpringBoard disposition.
        /// </summary>
        public PlistArray Disposition { get; }

        /// <summary>
        /// Gets the version of the SpringBoard disposition object.
        /// </summary>
        public string Version { get; }

        /// <inheritdoc/>
        public void Dispose() => Disposition.Dispose();
    }
}
