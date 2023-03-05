using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList
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

        /// <summary>
        /// Clone an item in the container.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected abstract object CloneItem(object item);

        IEnumerator IEnumerable.GetEnumerator() => CoreGetEnumerator();

        /// <summary>
        /// Get the <see cref="IEnumerator"/> for the container.
        /// </summary>
        /// <returns>An new instance of the enumerator</returns>
        protected abstract IEnumerator CoreGetEnumerator();
    }
}
