using IOSLib.PropertyList.Native;
using System;
using System.Collections.Generic;
using System.Text;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    public abstract class PlistNode : IOSLib.Native.IOSHandleWrapperBase<PlistHandle>,
        ICloneable,
        IEquatable<PlistNode>
    {

        public PlistNode(PlistHandle handle)
        {
            Handle = handle;
        }

        protected PlistNode()
        {
            
        }

        public PlistNode Clone()
        {
            return From(plist_copy(Handle));
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public PlistNode Parent
        {
            get
            {
                return PlistNode.From(plist_get_parent(Handle));
            }
        }


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

        public bool Equals(PlistNode other)
        {
            return this.Handle == other.Handle;
        }

        public override bool Equals(object obj)
        {
            if (typeof(PlistNode).IsAssignableFrom(obj.GetType()))
            {
                return Equals((PlistNode)obj);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Handle.GetHashCode();
        }

        public PlistType PlistType => plist_get_node_type(Handle);

    }
}
