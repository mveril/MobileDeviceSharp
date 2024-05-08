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
    /// <summary>
    /// Represent an application installed on a <see cref="IDevice"/>.
    /// </summary>
    public class Application : IDisposable
    {
        private readonly PlistDictionary _properties;

        /// <summary>
        /// Get a dictionary containing all the properties of the Application.
        /// </summary>
        public IReadOnlyDictionary<string, PlistNode> Properties => _properties;

        internal Application(IDevice device, PlistDictionary properties)
        {
            Device = device;
            _properties = properties;
        }

        /// <summary>
        /// Get device who contain the application.
        /// </summary>
        public IDevice Device { get; }

        /// <summary>
        /// Get the application Bundle Identifier.
        /// <para>
        ///     <seealso href="https://developer.apple.com/documentation/bundleresources/information_property_list/cfbundleidentifier">CFBundleIdentifier</seealso>.
        /// </para>
        /// </summary>
        public string BundleID => ((PlistString)Properties["CFBundleIdentifier"]).Value;

        /// <summary>
        /// Get the SpringBoard application tags that determine if the application can be visible on the home screen.
        /// </summary>
#if NET5_0_OR_GREATER
        public IReadOnlySet<string> SBAppTags
#else
        public IReadOnlyCollection<string> SBAppTags
#endif
        {
            get
            {
                if (Properties.TryGetValue("SBAppTags", out var plistValue) && plistValue is PlistArray appTagsArray)
                {
                    try
                    {
                        return new HashSet<string>(appTagsArray.Select(plist => ((PlistString)plist).Value));
                    }
                    finally
                    {
                        appTagsArray.Dispose();
                    }
                }
                else
                {
                    return new HashSet<string>();
                }
            }
        }

        public bool GetIsVisible(InstallationProxySession? externalSession = null)
        {
            if (Type == ApplicationType.Hidden)
            {
                return false;
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
                InstallationProxySession session = externalSession ?? new InstallationProxySession(Device);
                try
                {
                    var matcher = session.CheckCapabilityMatch(tags);
                    return matcher.Match;
                }
                finally
                {
                    // Dispose of the session only if it was created internally
                    if (externalSession == null)
                    {
                        externalSession?.Dispose();
                    }
                }
            }
        }


        /// <summary>
        /// Get a value which indicate if this application is visible on the home screen.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return GetIsVisible();
            }
        }

        /// <summary>
        /// Get the name of the application.
        /// <para>
        ///     <see href="https://developer.apple.com/documentation/bundleresources/information_property_list/cfbundlename">CFBundleName</see>
        /// </para>
        /// </summary>
        public string Name => ((PlistString)Properties["CFBundleName"]).Value;

        /// <summary>
        /// Get the display name of the application.
        /// <remark>
        ///   This is the name used by siri or on the home screen
        /// </remark>
        /// <para>
        ///     <see href="https://developer.apple.com/documentation/bundleresources/information_property_list/cfbundledisplayname">CFBundleDisplayName</see>
        /// </para>
        /// </summary>
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


        /// <summary>
        /// Get the type of this application.
        /// </summary>"ApplicationType"
        public ApplicationType Type
        {
            get
            {
                using PlistString plistType = (PlistString)Properties["ApplicationType"];
                var strType = plistType.Value;
                return (ApplicationType)Enum.Parse(typeof(ApplicationType), strType);
            }
        }

        /// <summary>
        /// Get the version of the app as a <see cref="System.Version"/> object.
        /// </summary>
        public Version? Version
        {
            get
            {
                Version? result = null;
                if (BundleVersion is null || !Version.TryParse(BundleVersion, out result))
                {
                    if (BundleShortVersion is not null)
                        _ = Version.TryParse(BundleShortVersion, out result);
                }
                return result;
            }
        }

        /// <summary>
        /// Get the full version of the application.
        /// <para>
        /// <seealso href="https://developer.apple.com/documentation/bundleresources/information_property_list/cfbundleshortversionstring">CFBundleShortVersionString</seealso>
        /// </para>
        /// </summary>
        public string? BundleShortVersion => Properties.TryGetValue("CFBundleShortVersionString", out var plistValue) ? ((PlistString)plistValue).Value : null;

        /// <summary>
        /// Get the full version of the application.
        /// <para>
        /// <seealso href="https://developer.apple.com/documentation/bundleresources/information_property_list/cfbundleversion">CFBundleShortVersionString</seealso>
        /// </para>
        /// </summary>
        public string? BundleVersion => Properties.TryGetValue("CFBundleVersion", out var plistValue) ? ((PlistString)plistValue).Value : null;

        /// <summary>
        /// Get a value which indicate if the application support file shairing is enable.
        /// <para>
        ///   This value indicate if the application can share document with a computer using <see href="https://support.apple.com//HT201301">iTunes</see>, the macOS <see href="https://support.apple.com/HT210598">Finder</see> (on macOS Catalina or greater) or fird party app.
        /// </para>
        /// <para>
        ///     <seealso href="https://developer.apple.com/documentation/bundleresources/information_property_list/uifilesharingenabled">UIFileSharingEnabled</seealso>
        /// </para>
        /// </summary>
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

        /// <summary>
        /// Get the path of the application.
        /// </summary>
        public string Path => ((PlistString)Properties["Path"]).Value;

        /// <summary>
        /// Get the path of the application container. 
        /// </summary>
        public string ContainerPath => ((PlistString)Properties["Container"]).Value;

        /// <summary>
        /// Uninstall the current application.
        /// </summary>
        /// <param name="progress">An <see cref="IProgress{Int32}"/> used to report the progress percentage.</param>
        /// <returns></returns>
        public async Task UninstallAsync(IProgress<int> progress)
        {
            using var session = new InstallationProxySession(Device);
            await session.UninstallAsync(BundleID, progress).ConfigureAwait(false);
        }

        /// <summary>
        /// Uninstall the current application.
        /// </summary>
        /// <returns></returns>
        public async Task UninstallAsync()
        {
            using var session = new InstallationProxySession(Device);
            await session.UninstallAsync(BundleID).ConfigureAwait(false);
        }

        /// <summary>
        /// Archive the current application.
        /// </summary>
        /// <param name="progress">An <see cref="IProgress{Int32}"/> used to report the progress percentage.</param>
        /// <returns></returns>
        public async Task ArchiveAsync(IProgress<int> progress)
        {
            var session = new InstallationProxySession(Device);
            await session.ArchiveAsync(BundleID, progress).ConfigureAwait(false);
        }


        /// <summary>
        /// Archive the current application.
        /// </summary>
        /// <returns></returns>
        public async Task ArchiveAsync()
        {
            var session = new InstallationProxySession(Device);
            await session.ArchiveAsync(BundleID).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _properties.Dispose();
        }
    }

}
