using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MobileDeviceSharp.PropertyList;
using MobileDeviceSharp.InstallationProxy;
using System.Diagnostics;

namespace MobileDeviceSharp.InstallationProxy
{
    public class Application
    {
        private readonly PlistDictionary _properties;
        public IReadOnlyDictionary<string, PlistNode> Properties => _properties;

        internal Application(IDevice device, PlistDictionary properties)
        {
            Device = device;
            _properties = properties;
        }

        public IDevice Device { get; }
        public string BundleID => ((PlistString)Properties["CFBundleIdentifier"]).Value;
#if NET5_0_OR_GREATER
        public IReadOnlySet<string> SBAppTags => ((PlistArray)Properties["SBAppTags"]).Select(node => ((PlistString)node).Value).ToHashSet();
#else
        public IReadOnlyCollection<string> SBAppTags => new HashSet<string>(((PlistArray)Properties["SBAppTags"]).Select(node => ((PlistString)node).Value));
#endif

        public bool IsVisible
        {
            get
            {
                if (Type == ApplicationType.Hidden)
                {
                    return true;
                }
#if NET5_0_OR_GREATER
                var tags = SBAppTags;
#else
                var tags = new HashSet<string>(SBAppTags);
#endif
                if (tags.Count == 0)
                {
                    return true;
                }
                else
                {
                    using var session = new InstallationProxySession(Device);
                    var matcher = session.CheckCapabilityMatch(tags);

                    return matcher.Match;
                }
            }
        }

        public string Name => ((PlistString)Properties["CFBundleName"]).Value;
        public string DisplayName
        {
            get
            {
                if (Properties.TryGetValue("CFBundleDisplayName", out var node) && node is PlistString displayNamePlist)
                {
                    return displayNamePlist.Value;
                }
                return Name;
            }
        } 


        public ApplicationType Type => (ApplicationType)Enum.Parse(typeof(ApplicationType), ((PlistString)Properties["ApplicationType"]).Value);

        public Version Version => Version.Parse(((PlistString)Properties["CFBundleShortVersionString"]).Value);

        public string BundleVersion => ((PlistString)Properties["CFBundleVersion"]).Value;

        public bool AllowFileShairing
        {
            get
            {
                if(Properties.TryGetValue("UIFileSharingEnabled", out var node) && node is PlistBoolean FileSharingNode)
                {
                    return FileSharingNode.Value;
                }
                else
                {
                    return false;
                }
            }
        }

        public string Path => ((PlistString)Properties["Path"]).Value;

        public string ContainerPath => ((PlistString)Properties["Container"]).Value;

        public ApplicationType AppType
        {
            get
            {
                using var typePlist = (PlistString)Properties["ApplicationType"];
                return (ApplicationType)Enum.Parse(typeof(ApplicationType), typePlist.Value);
            }
        }
            

        public async Task UninstallAsync(IProgress<int> progress)
        {
            using var session = new InstallationProxySession(Device);
            await session.UninstallAsync(BundleID, progress).ConfigureAwait(false);
        }

        public async Task UninstallAsync()
        {
            using var session = new InstallationProxySession(Device);
            await session.UninstallAsync(BundleID).ConfigureAwait(false);
        }

        public async Task ArchiveAsync(IProgress<int> progress)
        {
            var session = new InstallationProxySession(Device);
            await session.ArchiveAsync(BundleID, progress).ConfigureAwait(false);
        }



        public async Task ArchiveAsync()
        {
            var session = new InstallationProxySession(Device);
            await session.ArchiveAsync(BundleID).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _properties.Dispose();
        }
    }

}
