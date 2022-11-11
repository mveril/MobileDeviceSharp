using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.PropertyList;

namespace MobileDeviceSharp.InstallationProxy
{
    public class InstallationProxyArchiveOptions : InstallationProxyOperationOptions
    {
        public bool SkipUninstall { get; set; } = false;

        public ArchiveType ArchiveType { get; set; } = ArchiveType.All;
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
