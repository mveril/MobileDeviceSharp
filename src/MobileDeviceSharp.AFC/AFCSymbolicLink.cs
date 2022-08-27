using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.AFC
{
    public class AFCSymbolicLink : AFCItem
    {

        internal AFCSymbolicLink(AFCSessionBase session, string path) : base(session,path)
        {

        }

        protected override bool IsItemTypeSupported(AFCItemType itemType)
        {
            return itemType == AFCItemType.SymbolicLink;
        }

        public string ReadLink(bool recusive)
        {
            var TargetPath = GetFileInfo()["LinkTarget"];
            if (recusive)
            {
                var target = new AFCSymbolicLink(Session, TargetPath);
                if (target.Exists)
                {
                    return target.ReadLink(true);
                }
            }
            return TargetPath;
        }

        public string ReadLink()
        {
            return ReadLink(false);
        }
    }
}
