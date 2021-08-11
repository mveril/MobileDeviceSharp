using static IOSLib.PropertyList.Native.Plist;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IOSLib.PropertyList
{
    public partial class PlistDictionary
    {
        private class ValuesCollection : 
            ICollection<PlistNode>,
            IEnumerable<PlistNode>,
            IReadOnlyCollection<PlistNode>
        {
            private readonly PlistDictionary dict;

            public ValuesCollection(PlistDictionary plistDictionary)
            {
                this.dict = plistDictionary;
            }

            public int Count => throw new System.NotImplementedException();

            public bool IsReadOnly => throw new System.NotImplementedException();

            void ICollection<PlistNode>.Add(PlistNode item)
            {
                throw new System.NotSupportedException();
            }

            void ICollection<PlistNode>.Clear()
            {
                throw new System.NotSupportedException();
            }

            public bool Contains(PlistNode item)
            {
                return item.Parent.Equals(dict);
            }

            public void CopyTo(PlistNode[] array, int arrayIndex)
            {
                var i = arrayIndex;
                foreach (var item in this)
                {
                    array[i] = item.Clone();
                    i++;
                }
            }

            public IEnumerator<PlistNode> GetEnumerator()
            {
                return dict.Select(i => i.Value).GetEnumerator();
            }

            bool ICollection<PlistNode>.Remove(PlistNode item)
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