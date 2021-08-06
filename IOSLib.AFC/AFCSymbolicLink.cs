using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib.AFC
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
                var target = new AFCSymbolicLink(this.Session, TargetPath);
                if (target.Exist())
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
