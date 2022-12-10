using MobileDeviceSharp.PropertyList;

namespace MobileDeviceSharp.InstallationProxy
{
    /// <summary>
    /// Represent options for the process of looking applications installed on the device.
    /// </summary>
    public class InstalltionProxyLookupOptions : InstallationProxyOperationOptions
    {
        /// <summary>
        /// Create a new instance of <see cref="InstalltionProxyLookupOptions"/>.
        /// </summary>
        public InstalltionProxyLookupOptions()
        {
            ApplicationType = ApplicationType.Any;
        }

        /// <summary>
        /// Get application type to look up.
        /// </summary>
        public ApplicationType ApplicationType { get; set; }

        /// <summary>
        /// Search by Application Sinf
        /// </summary>
        public byte[]? ApplicationSinf { get; set; }

        /// <summary>
        /// Search by iTunes Metadata.
        /// </summary>
        public PlistNode? TunesMetadata { get; set; }

        /// <summary>
        /// Get or set a list of bundle ID to browse.
        /// </summary>
        public ISet<string> BundleIDs { get; } = new HashSet<string>();

        /// <inheritdoc/>
        public override PlistDictionary? ToDictionary()
        {
            PlistDictionary? dict = null;
            if (ApplicationType != ApplicationType.Any)
            {
                dict ??= new PlistDictionary();
                dict.Add("ApplicationType", new PlistString(Enum.GetName(ApplicationType.GetType(), ApplicationType)));
            }
            if (ApplicationSinf is not null)
            {
                dict ??= new PlistDictionary();
                dict.Add("ApplicationSinf", new PlistData(ApplicationSinf));
            }

            if (BundleIDs.Count == 1)
            {
                dict ??= new PlistDictionary();
                dict.Add("CFBundleIdentifier", new PlistString(BundleIDs.First()));
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

            if (BundleIDs.Count > 1)
            {
                dict ??= new PlistDictionary();
                dict.Add("BundleIDs", new PlistArray(BundleIDs.Select(s => new PlistString(s))));
            }
            return dict;
        }
    }
}
