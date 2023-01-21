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

        /// <inheritdoc/>
        public override PlistDictionary? ToDictionary()
        {
            PlistDictionary? dict = null;
            if (ApplicationType != ApplicationType.Any)
            {
                dict ??= new PlistDictionary();
                dict.Add("ApplicationType", new PlistString(Enum.GetName(ApplicationType.GetType(), ApplicationType)));
            }
            return dict;
        }
    }
}
