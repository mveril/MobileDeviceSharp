using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class ExceptionAttribute : Attribute
    {
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        public ExceptionAttribute(Type exceptionType)
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        {
            ExceptionType = exceptionType;
        }
        Type _ExceptionType;
        public Type ExceptionType
        {
            get
            {
                return _ExceptionType;
            }
            set
            {
                if (typeof(MobileDeviceException).IsAssignableFrom(value))
                {
                    _ExceptionType = value;
                }
            }
        }
    }
}
