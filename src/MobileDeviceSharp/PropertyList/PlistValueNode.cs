using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text;
using MobileDeviceSharp.PropertyList;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList
{
    /// <summary>
    /// Represent an abstract class for Plist node wich contain value.
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public abstract class PlistValueNode<T> : PlistNode where T : notnull
    {
        /// <summary>
        /// Represent a <see cref="PlistNode"/> containing a <see cref="Value"/>.
        /// </summary>
        /// <param name="handle"></param>
        protected PlistValueNode(PlistHandle handle) : base(handle)
        {

        }

        /// <summary>
        /// Get or set the value.
        /// </summary>
        public abstract T Value { get; set; }
        public bool Equals(PlistValueNode<T>? other)
        {
            return object.ReferenceEquals(this,other) || Handle.Equals(other?.Handle) || other == null ? false : Equals(other.Value);
        }

        public static explicit operator T(PlistValueNode<T> node) => node.Value;

    }
}
