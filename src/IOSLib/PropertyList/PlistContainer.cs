using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using IOSLib.PropertyList.Native;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    /// <summary>
    /// Represent a container of <see cref="PlistNode"/>.
    /// </summary>
    public abstract class PlistContainer : PlistNode,
        ICollection,
        IEnumerable
    {

        /// <inheritdoc/>
        public abstract int Count { get; }
        bool ICollection.IsSynchronized => false;
        private object _syncRoot = new();
        object ICollection.SyncRoot => _syncRoot;

        /// <summary>
        /// Get plist container from handle.
        /// </summary>
        /// <param name="handle"></param>
        public PlistContainer(PlistHandle handle) : base(handle)
        {

        }

        void ICollection.CopyTo(Array array, int index)
        {
            var i = index;
            foreach (var item in this)
            {
                array.SetValue(CloneItem(item), i);
                i++;
            }
        }

        protected abstract object CloneItem(object item);

        IEnumerator IEnumerable.GetEnumerator() => CoreGetEnumerator();
        protected abstract IEnumerator CoreGetEnumerator();
    }
}
