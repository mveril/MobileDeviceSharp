using System;
using MobileDeviceSharp.Native;
using System.Runtime.InteropServices;

namespace MobileDeviceSharp.CompilerServices
{
    /// <summary>
    /// This attribute is used to set a method used in the <see cref="SafeHandle.ReleaseHandle"/> method of the <see cref="IOSHandle"/> for the type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="IOSHandle"/> type that need this method to release</typeparam>.
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class UsedForReleaseAttribute<T> : Attribute where T: IOSHandle { }
}
