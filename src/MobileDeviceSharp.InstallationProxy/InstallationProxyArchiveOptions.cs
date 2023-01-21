using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.PropertyList;

namespace MobileDeviceSharp.InstallationProxy
{
    /// <summary>
    /// Represent options for the <see cref="InstallationProxySession.ArchiveAsync(string, InstallationProxyArchiveOptions?)"/> operation.
    /// </summary>
    public class InstallationProxyArchiveOptions : InstallationProxyOperationOptions
    {
        /// <summary>
        /// Get or set a value which indicate if the archiving operation should skip the uninstallation process.
        /// </summary>
        public bool SkipUninstall { get; set; } = false;

        /// <summary>
        /// Get or set which element should be archived.
        /// </summary>
        public ArchiveType ArchiveType { get; set; } = ArchiveType.All;

        /// <inheritdoc/>
        public override PlistDictionary? ToDictionary()
        {
            PlistDictionary? dic = null;
            if (ArchiveType != ArchiveType.All)
            {
                dic ??= new PlistDictionary();
                dic.Add("ArchiveType", new PlistString($"{ArchiveType}Only"));
            }
            if (SkipUninstall)
            {
                dic ??= new PlistDictionary();
                dic.Add("SkipUninstall", new PlistBoolean(SkipUninstall));
            }
            return dic;
        }
    }
}
