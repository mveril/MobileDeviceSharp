using MobileDeviceSharp.PropertyList;
using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp
{
    /// <summary>
    /// Represent the OS version information for an Apple device.
    /// </summary>
    public class OSVersion
    {
        internal static OSVersion FromDevice(IDevice idevice)
        {
            PlistDictionary dic;
            using (var relay = new DiagnosticsRelay.DiagnosticsRelaySession(idevice))
            {
                dic = relay.QueryMobilegestalt("MarketingProductName", "ProductVersion", "BuildVersion");
            }
            OSVersion oSVersion;
            using (dic)
            {
                var name = ((PlistString)dic["MarketingProductName"]).Value;
                var version = Version.Parse(((PlistString)dic["ProductVersion"]).Value);
                var build = BuildNumber.Parse(((PlistString)dic["BuildVersion"]).Value);
                oSVersion = new OSVersion(version, name, build);
            }
            return oSVersion;
        }

        private OSVersion(Version version, string oSDisplayName, BuildNumber buildNumber)
        {
            OSDisplayName = oSDisplayName;
            Version = version;
            BuildNumber = buildNumber;
        }

        private OSVersion(Version version, MobileDeviceSharp.DeviceClass deviceClass, BuildNumber buildNumber)
        {
            Version = version;
            BuildNumber = buildNumber;
            const string iOS = nameof(iOS);
            const string iPadOS = nameof(iPadOS);
            const string iPhoneOS = nameof(iPhoneOS);
            const string watchOS = nameof(watchOS);
            const string tvOS = nameof(tvOS);
            OSDisplayName = iOS;
            switch (deviceClass)
            {
                case DeviceClass.iPhone or DeviceClass.iPod:
                    if (Version.Major < 4)
                    {
                        OSDisplayName = iPhoneOS;
                    }
                    break;
                case DeviceClass.iPad:
                    if (Version.Major > 12)
                    {
                        OSDisplayName = iPadOS;
                    }
                    break;
                case DeviceClass.Watch:
                    OSDisplayName = watchOS;
                    break;
                case DeviceClass.AppleTV:
                    if (Version.Major > 8)
                    {
                        OSDisplayName = tvOS;
                    }
                    break;
            };
        }

        /// <summary>
        /// Get the display name of the operating system.
        /// </summary>
        public string OSDisplayName { get; }
        /// <summary>
        /// Get the version operating system.
        /// </summary>
        public Version Version { get; }
        /// <summary>
        /// Get the build number operating system.
        /// </summary>
        public BuildNumber BuildNumber { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{OSDisplayName} {Version}";
        }
        /// <summary>
        /// Get the string representation of the OSVersion
        /// </summary>
        /// <param name="showBuildNumber">Indicate if we need to show the build number</param>
        /// <returns></returns>
        public string ToString(bool showBuildNumber)
        {
            string str = ToString();
            if (showBuildNumber)
            {
                return $"{str} ({BuildNumber})";
            }
            return str;
        }
    }
}
