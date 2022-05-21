using System;
using System.ComponentModel;

namespace IOSLib
{
    /// <summary>
    /// Used to generate exception message for an hresult enum field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ExceptionMessageAttribute : Attribute
    {
        /// <summary>
        /// Define an exception message for the field with the specifed message.
        /// </summary>
        /// <param name="message">The message</param>
        public ExceptionMessageAttribute(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
