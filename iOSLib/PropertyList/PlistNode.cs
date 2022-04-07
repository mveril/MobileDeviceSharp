﻿using IOSLib.PropertyList.Native;
using System;
using System.Collections.Generic;
using System.Text;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    /// <summary>
    /// Represeent a node of a propery list.
    /// </summary>
    public abstract class PlistNode : IOSLib.Native.IOSHandleWrapperBase<PlistHandle>,
        ICloneable,
        IEquatable<PlistNode>
    {
        /// <summary>
        /// Initialize a <see cref="PlistNode"/> from the current handle.
        /// </summary>
        /// <param name="handle"></param>
        public PlistNode(PlistHandle handle)
        {
            Handle = handle;
        }

        protected PlistNode()
        {
            
        }

        /// <summary>
        /// Creates a new <see cref="PlistNode"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="PlistNode"/> that is a copy of this instance.</returns>
        public PlistNode Clone()
        {
            return From(plist_copy(Handle));
        }

        /// <inheritdoc/>
        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Get parent <see cref="PlistNode"/>.
        /// </summary>
        public PlistNode? Parent
        {
            get
            {
                var hParent = plist_get_parent(Handle);
                if (hParent.IsInvalid)
                {
                    return null;
                }
                else
                {
                    return PlistNode.From(hParent);
                }
            }
        }

        /// <summary>
        /// Get a plist node from handle.
        /// </summary>
        /// <param name="plistHandle">The handle to wrap.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Occure when the type of the PlistNode is unknow</exception>
        public static PlistNode From(PlistHandle plistHandle)
        {
            return plist_get_node_type(plistHandle) switch
            {
                PlistType.Dict => new PlistDictionary(plistHandle),
                PlistType.Array => new PlistArray(plistHandle),
                PlistType.Boolean => new PlistBoolean(plistHandle),
                PlistType.Uint => new PlistInteger(plistHandle),
                PlistType.Real => new PlistReal(plistHandle),
                PlistType.String => new PlistString(plistHandle),
                PlistType.Key => new PlistKey(plistHandle),
                PlistType.Uid => new PlistUid(plistHandle),
                PlistType.Date => new PlistDate(plistHandle),
                PlistType.Data => new PlistData(plistHandle),
                _ => throw new NotSupportedException()
            };
        }

        /// <inheritdoc/>
        public bool Equals(PlistNode other)
        {
            return this.Handle == other.Handle;
        }
        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (typeof(PlistNode).IsAssignableFrom(obj.GetType()))
            {
                return Equals((PlistNode)obj);
            }
            return base.Equals(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.Handle.GetHashCode();
        }
        /// <summary>
        /// Get the type of the current Plist node.
        /// </summary>
        public PlistType PlistType => plist_get_node_type(Handle);

    }
}
