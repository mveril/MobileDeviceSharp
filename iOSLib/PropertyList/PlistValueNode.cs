using System;
using System.Collections.Generic;
using System.Text;
using IOSLib.PropertyList.Native;

namespace IOSLib.PropertyList
{
    public abstract class PlistValueNode<T> : PlistNode
    {
        protected PlistValueNode(PlistHandle handle) : base(handle)
        {

        }

        protected PlistValueNode()
        {

        }

        public abstract T Value { get; set; }
    }
}
