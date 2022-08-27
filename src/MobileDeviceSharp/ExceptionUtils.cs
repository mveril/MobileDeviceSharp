using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MobileDeviceSharp
{
    public static class ExceptionUtils
    {
        public static MobileDeviceException? GetException<T>(T value) where T : Enum
        {
            var attributes = typeof(T).GetCustomAttribute<ExceptionAttribute>(true);

            if (attributes is not null)
            {
                if ((int)(object)value == 0)
                {
                    return null;
                }
                else
                {
                    return (MobileDeviceException)Activator.CreateInstance(attributes.ExceptionType, value);
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
