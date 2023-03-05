using System;

namespace MobileDeviceSharp.CompilerServices
{
    /// <summary>
    /// Allow to generate an Handle for libimobiledevice.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class GenerateHandleAttribute : Attribute
    {
        /// <summary>
        /// The name of the handle
        /// </summary>
        public string HandleName { get; }

        /// <summary>
        /// Initialize a new instance of the <see cref="GenerateHandleAttribute"/>
        /// </summary>
        public GenerateHandleAttribute() : base()
        {

        }

        /// <summary>
        /// /// Initialize a new instance of the <see cref="GenerateHandleAttribute"/> with the specified <paramref name="handleName"/>.
        /// </summary>
        /// <param name="handleName">The name of the handle.</param>
        public GenerateHandleAttribute(string handleName) : base()
        {
            HandleName = handleName;
        }
    }
}
