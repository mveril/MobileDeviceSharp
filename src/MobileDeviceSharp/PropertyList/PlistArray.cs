using MobileDeviceSharp.PropertyList.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList
{
    /// <summary>
    /// Represent a PropertyList array
    /// </summary>
    public sealed partial class PlistArray : PlistContainer,
        IReadOnlyCollection<PlistNode>,
        ICollection<PlistNode>,
        IEnumerable<PlistNode>,
        IReadOnlyList<PlistNode>,
        IList<PlistNode>
    {
        /// <summary>
        /// Create Array plist node from an existing handle.
        /// </summary>
        /// <param name="node">The <see cref="PlistHandle"/> of type <see cref="PlistType.Array"/> to wrap.</param>
        public PlistArray(PlistHandle node) : base(node)
        {

        }
        /// <summary>
        /// Create a new PlistArray
        /// </summary>
        public PlistArray() : base(plist_new_array())
        {

        }

        /// <summary>
        /// Create a new PlistArray from list of nodes
        /// </summary>
        /// <param name="source">The nodes to add to the array</param>
        public PlistArray(IEnumerable<PlistNode> source) : this()
        {
            foreach (var item in source)
            {
                Add(item);
            }
        }

        /// <inheritdoc/>
        public PlistNode this[int index]
        {
            get
            {
                var childHandle = plist_array_get_item(Handle, (uint)index);
                if (childHandle.IsInvalid && !Handle.IsInvalid)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return From(childHandle)!;
            }
            set
            {
                plist_array_set_item(Handle, plist_copy_not_owned(value.Handle), (uint)index);
            }
        }

        /// <inheritdoc/>
        public override int Count => (int)plist_array_get_size(Handle);


        bool ICollection<PlistNode>.IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(PlistNode item)
        {
            plist_array_append_item(Handle, plist_copy_not_owned(item.Handle));
        }

        /// <inheritdoc/>
        public void Clear()
        {
            var count = Count;
            for (int i = count - 1; i < 0; i--)
            {
                RemoveAt(i);
            }
        }
        /// <inheritdoc/>
        public bool Contains(PlistNode item)
        {
            return plist_array_get_item_index(item.Handle) != uint.MaxValue;
        }

        /// <inheritdoc/>
        public void CopyTo(PlistNode[] array, int arrayIndex)
        {
            var i = arrayIndex;
            foreach (var item in this)
            {
                array[i] = item;
                i++;
            }
        }


        protected override IEnumerator CoreGetEnumerator() => new Enumerator(this);

        protected override object CloneItem(object item) => ((PlistNode)item).Clone();

        /// <inheritdoc/>
        public int IndexOf(PlistNode item)
        {
            var index = plist_array_get_item_index(item.Handle);
            if (index == uint.MaxValue)
            {
                return -1;
            }
            else
            {
                return (int)index;
            }
        }

        /// <inheritdoc/>
        public void Insert(int index, PlistNode item)
        {
            plist_array_insert_item(Handle, item.Clone().Handle, (uint)index);
        }
        /// <inheritdoc/>
        public bool Remove(PlistNode item)
        {
            var result = item.Parent?.Handle == Handle;
            if (result)
            {
                plist_array_item_remove(item.Handle);
            }
            return result;
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            plist_array_remove_item(Handle, (uint)index);
        }
        /// <inheritdoc/>
        public IEnumerator<PlistNode> GetEnumerator() => new Enumerator(this);
    }
}
