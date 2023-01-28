using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.HouseArrest.Native
{
    [Exception(typeof(HouseArrestException))]
    public enum HouseArrestError : int
    {

        Success = 0,

        InvalidArg = -1,

        PlistError = -2,

        ConnFailed = -3,

        InvalidMode = -4,

        UnknownError = -256,
    }
}
