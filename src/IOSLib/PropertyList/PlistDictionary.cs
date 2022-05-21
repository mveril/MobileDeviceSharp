using IOSLib.PropertyList.Native;
using static IOSLib.PropertyList.Native.Plist;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System;

namespace IOSLib.PropertyList
{
    /// <summary>
    /// Represent a property list dictionary
    /// </summary>
    public sealed partial class PlistDictionary : PlistContainer,
        IDictionary<string,PlistNode>,
        ICollection<KeyValuePair<string,PlistNode>>,
        IEnumerable<KeyValuePair<string, PlistNode>>
    {
        /// <inheritdoc/>
        public ICollection<string> Keys => new KeysCollection(this);

        /// <inheritdoc/>
        public ICollection<PlistNode> Values => new ValuesCollection(this);

        /// <inheritdoc/>
        public override int Count => (int)plist_dict_get_size(Handle);

        bool ICollection<KeyValuePair<string, PlistNode>>.IsReadOnly => true;
        
        /// <inheritdoc/>
        public PlistNode this[string key] 
        {
            get
            {
                var item = From(plist_dict_get_item(Handle, key));
                if (item == null)
                {
                    throw new KeyNotFoundException($"The key {key} is not found in this dictionary");
                }
                return item;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Handle.IsClosed)
                    throw new ObjectDisposedException(nameof(value));
                plist_dict_set_item(Handle, key, value.Handle);
            }
        }
        /// <summary>
        /// /// Create plist dictionary from an existing handle.
        /// </summary>
        /// <param name="node">The <see cref="PlistHandle"/> of type <see cref="PlistType.Dict"/> to wrap.</param>
        public PlistDictionary(PlistHandle node) : base(node)
        {

        }

        /// <summary>
        /// Create an empty
        /// </summary>
        public PlistDictionary() : base(plist_new_dict())
        {

        }

        /// <summary>
        /// Create plist dictionary from a dictionary of nodes
        /// </summary>
        /// <param name="source"></param>
        public PlistDictionary(IReadOnlyDictionary<string,PlistNode> source) : this()
        {
            foreach (var item in source)
            {
                Add(item);
            }
        }
        /// <inheritdoc/>
        public void Add(string key, PlistNode value)
        {
            if (value.Handle.IsClosed)
                throw new ObjectDisposedException(nameof(value));
            plist_dict_insert_item(Handle, key, value.Clone().Handle);
        }

        /// <inheritdoc/>
        public bool ContainsKey(string key)
        {
            return !plist_dict_get_item(Handle, key).IsInvalid;
        }

        /// <inheritdoc/>
        public bool Remove(string key)
        {
            try
            {
                plist_dict_remove_item(Handle, key);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
            
        }

        /// <inheritdoc/>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        public bool TryGetValue(string key, [MaybeNullWhen(false)] out PlistNode value)
#else
        public bool TryGetValue(string key, out PlistNode value)
#endif
        {
            value = From(plist_dict_get_item(Handle, key));
            return value is not null;
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<string, PlistNode> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            var keyscopy = Keys.ToList();
            foreach  (string key in keyscopy)
            {
                Remove(key);
            }
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<string, PlistNode> item)
        {
            var c = TryGetValue(item.Key, out var val);
            if (c)
            {
                return val.Handle == item.Value.Handle;
            }
            return false;
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<string, PlistNode>[] array, int arrayIndex)
        {
            var i = arrayIndex;
            foreach (var item in this)
            {

                array[i] = (KeyValuePair<string, PlistNode>)CloneItem(item);
                i++;
            }
        }

        protected override object CloneItem(object item)
        {
            var titem = (KeyValuePair<string, PlistNode>)item;
            return new KeyValuePair<string, PlistNode>(titem.Key, titem.Value.Clone());
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<string, PlistNode> item)
        {
            return Remove(item.Key);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, PlistNode>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <inheritdoc/>
        protected override IEnumerator CoreGetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
