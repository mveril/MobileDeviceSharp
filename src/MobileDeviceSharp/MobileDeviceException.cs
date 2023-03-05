using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileDeviceSharp
{
    /// <summary>
    /// An exception thrown by the imobiledevice library.
    /// </summary>
    public abstract class MobileDeviceException : ExternalException
    {
        /// <summary>
        /// Initialize a new instace of the <see cref="MobileDeviceException"/>.
        /// </summary>
        public MobileDeviceException() : base() { }

        /// <summary>
        /// Initialize a new instace of the <see cref="MobileDeviceException"/> with the specified <paramref name="errorCode"/> and <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
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
