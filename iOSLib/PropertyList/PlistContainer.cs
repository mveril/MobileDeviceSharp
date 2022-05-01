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
        private object _syncRoot = new();
        object ICollection.SyncRoot => _syncRoot;

        /// <summary>
        /// Get plist container from handle.
        /// </summary>
        /// <param name="handle"></param>
        public PlistContainer(PlistHandle handle) : base(handle)
        {

        }

        /// <summary>
        /// Get XML <see cref="string"/> from the plist.
        /// </summary>
        /// <returns>XML <see cref="string"/></returns>
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
        /// <returns>Binary plist as byte array</returns>
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
        public static PlistContainer? FromXml(string xml)
        {
            int length = Encoding.UTF8.GetByteCount(xml);
            plist_from_xml(xml, (uint)length, out var handle);
            var node = From(handle);
            if (node is PlistContainer container)
            {
                container.IsBinary = false;
                return container;
            }
            return null;
        }

        public static PlistContainer? FromBin(byte[] bin)
        {
            uint length = (uint)bin.Length;
            plist_from_bin(bin, length, out var handle);
            using var node = From(handle);
            if (node is PlistContainer container)
            {
                container.IsBinary = true;
                return container;
            }
            return null;
        }

        /// <summary>
        /// Get plist data from file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The plist container</returns>
        public static PlistContainer? FromFile(string path)
        {
            using var fileStream = new FileStream(path, FileMode.Open);
            return FromStream(fileStream);
        }

        /// <summary>
        /// Get plist data from file path.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <returns>The plist container</returns>
        public static PlistContainer? FromStream(Stream stream)
        {
            PlistContainer? container;

            uint leight = (uint)stream.Length;
            unsafe
            {
                byte* ptr;
#if NET6_0_OR_GREATER
                ptr = (byte*)NativeMemory.Alloc(leight);
#else
                ptr = (byte*)Marshal.AllocHGlobal((nint)leight).ToPointer();
#endif
                var ums = new UnmanagedMemoryStream(ptr, stream.Length,stream.Length, FileAccess.Write);
                stream.CopyTo(ums);
                stream.Close();
                plist_from_memory(ptr, leight, out var handle);
                var node = From(handle);{
                container = node as PlistContainer;
                if (container is not null)
                    container.IsBinary = plist_is_binary(ptr, leight) != 0;
                }

#if NET6_0_OR_GREATER
                NativeMemory.Free(ptr);
#else
                Marshal.FreeHGlobal((IntPtr)ptr);
#endif             
            }
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
