using IOSLib.PropertyList.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    public sealed partial class PlistArray : PlistContainer,
        ICollection<PlistNode>,
        IEnumerable<PlistNode>,
        IList<PlistNode>
    {

        public PlistArray(PlistHandle node) : base(node)
        {

        }

        public PlistArray() : base(plist_new_array())
        {

        }

        public PlistArray(IEnumerable<PlistNode> source) : this()
        {
            foreach (var item in source)
            {
                this.Add(item);
            }
        }

        public PlistNode this[int index]
        {
            get
            {
                return PlistNode.From(plist_array_get_item(Handle, (uint)index));
            }
            set
            {
                plist_array_set_item(Handle, value.Clone().Handle, (uint)index);
            }
        }

        public override int Count => (int)plist_array_get_size(Handle);

        bool ICollection<PlistNode>.IsReadOnly => false;

        public void Add(PlistNode item)
        {
            plist_array_append_item(Handle, item.Clone().Handle);
        }

        public void Clear()
        {
            var count = Count;
            for (int i = count - 1; i < 0; i--)
            {
                RemoveAt(i);
            }
        }

        public bool Contains(PlistNode item)
        {
            return plist_array_get_item_index(item.Handle) != uint.MaxValue;
        }

        public void CopyTo(PlistNode[] array, int arrayIndex)
        {
            var i = arrayIndex;
            foreach (var item in this)
            {
                array[i]=item;
                i++;
            }
        }

        protected override IEnumerator CoreGetEnumerator() => new Enumerator(this);

        protected override object CloneItem(object item) => ((PlistNode)item).Clone();

        public int IndexOf(PlistNode item)
        {
            var index = plist_array_get_item_index(item.Handle);
            if (index== uint.MaxValue)
            {
                return -1;
            }
            else
            {
                return (int)index;
            }
        }

        public void Insert(int index, PlistNode item)
        {
            plist_array_insert_item(Handle, item.Clone().Handle, (uint)index);
        }

        public bool Remove(PlistNode item)
        {
            try
            {
                var index = IndexOf(item);
                if (index== -1)
                {
                    return false;
                }
                else
                {
                    RemoveAt(index);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void RemoveAt(int index)
        {
            plist_array_remove_item(Handle, (uint)index);
        }

        public IEnumerator<PlistNode> GetEnumerator() => new Enumerator(this);
    }
}
