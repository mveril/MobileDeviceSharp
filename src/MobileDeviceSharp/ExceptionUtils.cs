using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MobileDeviceSharp
{
    /// <summary>
    /// Utilities to get <see cref="MobileDeviceException"/>.
    /// </summary>
    public static class ExceptionUtils
    {
        /// <summary>
        /// Get <see cref="MobileDeviceException"/> from enum value of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of enum error.</typeparam>
        /// <param name="value">The enum value of type <typeparamref name="T"/>.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Raised when the typ <typeparamref name="T"/> not correspond to MobileDevice Error.</exception>
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
