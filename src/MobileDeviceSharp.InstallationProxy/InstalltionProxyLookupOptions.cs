using MobileDeviceSharp.PropertyList;

namespace MobileDeviceSharp.InstallationProxy
{
    public class InstalltionProxyLookupOptions : InstallationProxyOperationOptions
    {
        public InstalltionProxyLookupOptions()
        {
            ApplicationType = ApplicationType.Any;
        }

        public ApplicationType ApplicationType { get; set; }

        public string? CfBundleIdentifier { get; set; }

        public byte[]? ApplicationSinf { get; set; }

        public PlistNode? TunesMetadata { get; set; }

        public string[]? BundleIDs { get; set; }

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

            if (CfBundleIdentifier is not null)
            {
                dict ??= new PlistDictionary();
                dict.Add("CFBundleIdentifier", new PlistString(CfBundleIdentifier));
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

            if (BundleIDs is not null)
            {
                dict ??= new PlistDictionary();
                dict.Add("BundleIDs", new PlistArray(BundleIDs.Select(s => new PlistString(s))));
            }
            return dict;
        }
    }
}
