using PlistSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib
{
    public class OSVersion
    {
        static internal OSVersion FromDevice(IDevice idevice)
        {
            string buildNumber;
            string sversion;
            using (var lockdown = new Lockdown(idevice))
            {
                using (var pValue = (PlistString)lockdown.GetValue("BuildVersion"))
                {
                    buildNumber = pValue.Value;
                }
                using (var pValue = (PlistString)lockdown.GetValue("ProductVersion"))
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
                case DeviceClass.iPhone or IOSLib.DeviceClass.iPodTouch:
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

        public string OSDisplayName { get; }
        public Version Version { get; }
        public BuildNumber BuildNumber { get; }
        public override string ToString()
        {
            return $"{OSDisplayName} {Version}";
        }
        public string ToString(bool ShowBuildNumber)
        {
            string? str = ToString();
            if (ShowBuildNumber)
            {
                return str + $" ({BuildNumber})";
            }
            return str;
        }
    }
}
