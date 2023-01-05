using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.PropertyList.Native;

namespace MobileDeviceSharp.PropertyList
{
    /// <summary>
    /// Represent an abstract class for Plist node wich contain value.
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public abstract class PlistValueNode<T> : PlistNode where T : notnull
    {
        protected PlistValueNode(PlistHandle handle) : base(handle)
        {

        }

        /// <summary>
        /// Get or set the value.
        /// </summary>
        public abstract T Value { get; set; }

        public static explicit operator T(PlistValueNode<T> node) => node.Value;

    }
}
