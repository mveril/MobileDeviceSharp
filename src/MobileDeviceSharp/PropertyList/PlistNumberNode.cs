#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text;
using MobileDeviceSharp.PropertyList.Native;

namespace MobileDeviceSharp.PropertyList
{
    public abstract class PlistNumberNode<T> : PlistValueNode<T> where T : notnull, INumber<T>
    {

    }
}
#endif
