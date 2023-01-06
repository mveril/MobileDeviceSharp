using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.PropertyList;

namespace MobileDeviceSharp.InstallationProxy
{
    public sealed class InstallationProxyInstallOptions : InstallationProxyOperationOptions
    {
        /// <summary>
        /// Provide by Application Sinf
        /// </summary>
        public byte[]? ApplicationSinf { get; set; }

        /// <summary>
        /// Provide by iTunes Metadata.
        /// </summary>
        public PlistDictionary TunesMetadata { get; }

        /// <summary>
        /// If PackageType -> Developer is specified, then the provided path need to point to
        /// an.app directory instead of an install package.
        /// </summary>
        public bool IsDevelopperPackage { get; }

        /// <inheritdoc/>
        public override PlistDictionary? ToDictionary()
        {
            PlistDictionary? dict = null;
            if (ApplicationSinf is not null)
            {
                dict ??= new PlistDictionary();
                dict.Add("ApplicationSinf", new PlistData(ApplicationSinf));
            }

            if (TunesMetadata is not null)
            {
                dict ??= new PlistDictionary();
                dict.Add("iTunesMetadata", TunesMetadata);
            }

            if (ApplicationSinf is not null)
            {
                dict ??= new PlistDictionary();
                dict.Add("ApplicationSinf", new PlistData(ApplicationSinf));
            }

            if (IsDevelopperPackage)
            {
                dict ??= new PlistDictionary();
                dict.Add("PackageType", new PlistString("Developer"));
            }
            return dict;
        }
    }
}
