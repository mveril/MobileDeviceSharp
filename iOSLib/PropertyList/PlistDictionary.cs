using IOSLib.PropertyList.Native;
using static IOSLib.PropertyList.Native.Plist;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System;

namespace IOSLib.PropertyList
{
    public sealed partial class PlistDictionary : PlistContainer,
        IDictionary<string,PlistNode>,
        ICollection<KeyValuePair<string,PlistNode>>,
        IEnumerable<KeyValuePair<string, PlistNode>>
    {
        public ICollection<string> Keys => new KeysCollection(this);

        public ICollection<PlistNode> Values => new ValuesCollection(this);

        public override int Count => (int)plist_dict_get_size(Handle);

        bool ICollection<KeyValuePair<string, PlistNode>>.IsReadOnly => true;

        public PlistNode this[string key] 
        {
            get
            {
                return From(plist_dict_get_item(Handle, key));
            }
            set 
            {
                plist_dict_set_item(Handle, key, value.Handle);
            }
        }

        public PlistDictionary(PlistHandle node) : base(node)
        {

        }

        public PlistDictionary() : base(plist_new_dict())
        {

        }

        public PlistDictionary(IReadOnlyDictionary<string,PlistNode> source) : this()
        {
            foreach (var item in source)
            {
                this.Add(item);
            }
        }

        public void Add(string key, PlistNode value)
        {
            plist_dict_insert_item(Handle, key, value.Clone().Handle);
        }

        public bool ContainsKey(string key)
        {
            return !plist_dict_get_item(Handle, key).IsInvalid;
        }

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

        public bool TryGetValue(string key, out PlistNode value)
        {
            var iHandle = plist_dict_get_item(Handle, key);
            if (!iHandle.IsInvalid)
            {
                value = PlistNode.From(iHandle);
                return true;
            }
            value = null!;
            return false;
        }

        public void Add(KeyValuePair<string, PlistNode> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, PlistNode> item)
        {
            var c = TryGetValue(item.Key, out var val);
            if (c)
            {
                return val.Handle == item.Value.Handle;
            }
            return false;
        }

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

        public bool Remove(KeyValuePair<string, PlistNode> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, PlistNode>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        protected override IEnumerator CoreGetEnumerator()
        {
            return GetEnumerator();
        }
    }
}