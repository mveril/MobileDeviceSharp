using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace IOSLib
{
    public abstract class MobileDeviceException : ExternalException
    {
        public MobileDeviceException() : base() { }
        public MobileDeviceException(string message, int errorCode) : base(message,errorCode) { }

        protected static string GetMessageForHResult<T>(T value) where T : Enum
        {
            var enumType = typeof(T);
            var memberData = enumType.GetMember(value.ToString()).First();
            var Description = memberData.GetCustomAttributes<ExceptionMessageAttribute>(false).FirstOrDefault()?.Message;
            if (Description==null)
            {
                return value.ToString();
            }
            return Description;
        }
    }
}
