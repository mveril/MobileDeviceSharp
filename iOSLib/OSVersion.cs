using IOSLib.PropertyList;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib
{
    /// <summary>
    /// Represent the OS version information for an Apple device.
    /// </summary>
    public class OSVersion
    {
        internal static OSVersion FromDevice(IDevice idevice)
        {
            string buildNumber;
            string sversion;
            using (var lockdown = new LockdownSession(idevice))
            {
                var domain = lockdown.GetDomain();
                using (var pValue = (PlistString)domain["BuildVersion"])
                {
                    buildNumber = pValue.Value;
                }
                using (var pValue = (PlistString)domain["ProductVersion"])
                {
                    sversion = pValue.Value;
                }
            }
            var version = Version.Parse(sversion);
            var deviceClass = idevice.DeviceClass;
            return new OSVersion(version, deviceClass, new BuildNumber(buildNumber));
        }
        private OSVersion(Version version, IOSLib.DeviceClass deviceClass, BuildNumber buildNumber)
        {
            Version = version;
            BuildNumber = buildNumber;
            const string iOS = nameof(iOS);
            const string iPadOS = nameof(iPadOS);
            const string iPhoneOS = nameof(iPhoneOS);
            OSDisplayName = iOS;
            switch (deviceClass)
            {
                case DeviceClass.iPhone or DeviceClass.iPodTouch:
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
            string? str = ToString();
            if (showBuildNumber)
            {
                return str + $" ({BuildNumber})";
            }
            return str;
        }
    }
}
