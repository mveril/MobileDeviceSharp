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
        /// <summary>
        /// CHeck if the Plist is binary.
        /// </summary>
        public bool IsBinary { get; private set; }

        /// <inheritdoc/>
        public abstract int Count { get; }
        bool ICollection.IsSynchronized => false;
        private object _syncRoot = new object();
        object ICollection.SyncRoot => _syncRoot;

        /// <summary>
        /// Get plist container from handle
        /// </summary>
        /// <param name="handle"></param>
        public PlistContainer(PlistHandle handle) : base(handle)
        {

        }

        /// <summary>
        /// Get XML <see cref="string"/> from the plist.
        /// </summary>
        /// <returns>XML string</returns>
        public string ToXML()
        {
            plist_to_xml(Handle, out var xmlptr, out var leight);
            string str;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP1_1_OR_GREATER
            str = Marshal.PtrToStringUTF8(xmlptr, (int)leight);
#else

            unsafe
            {
                str = Encoding.UTF8.GetString((byte*)xmlptr, (int)leight);
            }
#endif
            plist_to_xml_free(xmlptr);
            return str;
        }

        /// <summary>
        /// Get binary plist from data.
        /// </summary>
        /// <returns></returns>
        public byte[] ToBin()
        {
            plist_to_bin(Handle, out IntPtr ptr, out var length);
            byte[] buffer = new byte[length];
            Marshal.Copy(ptr, buffer, 0, (int)length);
            plist_to_bin_free(ptr);
            return buffer;
        }

        /// <summary>
        /// Get plist from XML string.
        /// </summary>
        /// <param name="xml">XML string</param>
        /// <returns>PlistContainer</returns>
        public static PlistContainer FromXml(string xml)
        {
            int length = Encoding.UTF8.GetByteCount(xml);
            plist_from_xml(xml, (uint)length, out var handle);
            var container = (PlistContainer)PlistNode.From(handle);
            container.IsBinary = false;
            return container;
        }

        private static unsafe PlistContainer FromBin(byte[] bin)
        {
            uint length = (uint)bin.Length;
            plist_from_bin(bin, length, out var handle);
            var structure = (PlistContainer)PlistNode.From(handle);
            structure.IsBinary = plist_is_binary(bin, length) != 0;
            return structure;
        }

        /// <summary>
        /// Get plist data from file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The plist container</returns>
        public static PlistContainer FromFile(string path)
        {
            using FileStream fileStream = new FileStream(path, FileMode.Open);
            return FromStream(fileStream);
        }

        /// <summary>
        /// Get plist data from file path.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <returns>The plist container</returns>
        public static unsafe PlistContainer FromStream(Stream stream)
        {
            byte[] buffer;
            var length = stream.Length;
            if (stream is MemoryStream)
            {
                buffer = ((MemoryStream)stream).GetBuffer();
            }
            else
            {
                buffer = new byte[length];
                var memoryStream = new MemoryStream(buffer);
                stream.CopyTo(memoryStream);
            }
            plist_from_memory(buffer, (uint)length, out var handle);
            var container = (PlistContainer)PlistNode.From(handle);
            container.IsBinary = plist_is_binary(buffer, (uint)length) != 0;
            return container;
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
