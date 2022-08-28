using System;

namespace MobileDeviceSharp.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class GenerateHandleAttribute : Attribute
    {
        public string HandleName { get; }

        public GenerateHandleAttribute() : base()
        {

        }

        public GenerateHandleAttribute(string handleName) : base()
        {
            HandleName = handleName;
        }

    }
}
