using System;
using System.ComponentModel;

namespace IOSLib
{

    [AttributeUsage(AttributeTargets.Field)]
    public class ExceptionMessageAttribute : Attribute
    {
        public ExceptionMessageAttribute(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}