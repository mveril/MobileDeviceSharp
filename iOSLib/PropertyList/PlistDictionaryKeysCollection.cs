using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IOSLib.PropertyList
{
    public partial class  PlistDictionary
    { 
    private class KeysCollection : 
            ICollection<string>,
            IEnumerable<string>,
            IReadOnlyCollection<string>
    {
        private readonly PlistDictionary dict;

        public KeysCollection(PlistDictionary plistDictionary)
        {
            this.dict = plistDictionary;
        }

        public int Count => dict.Count;

        public bool IsReadOnly => true;

        void ICollection<string>.Add(string item)
        {
            throw new System.NotSupportedException();
        }

        void ICollection<string>.Clear()
        {
            throw new System.NotSupportedException();
        }

        public bool Contains(string item)
        {
            return dict.ContainsKey(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            var i = arrayIndex;
            foreach (var item in this)
            {
                array[i] = item;
                i++;
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return dict.Select(i => i.Key).GetEnumerator();
        }

        bool ICollection<string>.Remove(string item)
        {
            throw new System.NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    }
}