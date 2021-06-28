using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib.AFC
{
    class AFC2Session : AFCSessionBase
    {
        private const string AFC2_SERVICE_ID = "com.apple.afc2";

        AFC2Session(IDevice device) : base(device, AFC2_SERVICE_ID) { }

        public override string Root => "/";
    }
}
