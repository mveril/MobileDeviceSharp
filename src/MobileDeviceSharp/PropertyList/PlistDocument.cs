using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList
{
    /// <summary>
    /// Represent a Property list document
    /// </summary>
    public sealed class PlistDocument : IDisposable
    {
        /// <summary>
        /// Create a new Plist Document with the specified root node and the specified format
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="format"></param>
        public PlistDocument(PlistNode rootNode, PlistDocumentFormats format)
        {
            RootNode = rootNode;
            Format = format;
        }

        /// <summary>
        /// Get the root node of this Plist document
        /// </summary>
        public PlistNode RootNode { get; }

        /// <summary>
        /// Get or set the default format of this document.
        /// </summary>
        public PlistDocumentFormats Format { get; set; }


        /// <summary>
        /// Get XML <see cref="string"/> from the plist.
        /// </summary>
        /// <returns>XML <see cref="string"/></returns>
        public string ToXMLString()
        {
            IntPtr xmlptr = IntPtr.Zero;
            try
            {
                plist_to_xml(RootNode.Handle, out xmlptr, out var leight);
                string str;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP1_1_OR_GREATER
            str = Marshal.PtrToStringUTF8(xmlptr, (int)leight);
#else

                unsafe
                {
                    str = Encoding.UTF8.GetString((byte*)xmlptr, (int)leight);
                }
#endif
                return str;
            }
            finally
            {
                plist_to_xml_free(xmlptr);
            }
        }

        /// <summary>
        /// Get binary plist from data.
        /// </summary>
        /// <returns>Binary plist as byte array</returns>
        public byte[] ToBin()
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                plist_to_bin(RootNode.Handle, out ptr, out var length);
                byte[] buffer = new byte[length];
                Marshal.Copy(ptr, buffer, 0, (int)length);
                return buffer;
            }
            finally
            {
                plist_to_bin_free(ptr);
            }
        }

        /// <summary>
        /// Get a Plist document from XML string.
        /// </summary>
        /// <param name="xml">XML string</param>
        /// <returns>The plist document</returns>
        public static PlistDocument? FromXmlString(string xml)
        {
            int length = Encoding.UTF8.GetByteCount(xml);
            plist_from_xml(xml, (uint)length, out var handle);
            var node = PlistNode.From(handle);
            if (node is not null)
            {
                return new PlistDocument(node, PlistDocumentFormats.XML);
            }
            return null;
        }

        /// <summary>
        /// Get a Plist document from XML string.
        /// </summary>
        /// <param name="xml">XML string</param>
        /// <returns>The plist document</returns>
        public static PlistDocument? FromXmlString(ReadOnlySpan<char> xml)
        {
            Native.PlistHandle handle = Native.PlistHandle.Zero;
            unsafe
            {
                byte[]? arr = null;
                int length = 0;
                fixed (char* cptr = xml)
                {
                    length = Encoding.UTF8.GetByteCount(cptr, xml.Length);
                    arr = ArrayPool<byte>.Shared.Rent(length);
                    fixed (byte* bptr = arr)
                    {
                        Encoding.UTF8.GetBytes(cptr, xml.Length,bptr,arr.Length);
                    }
                    plist_from_xml(arr, (uint)length, out handle);
                    ArrayPool<byte>.Shared.Return(arr, true);
                }
            }

            var node = PlistNode.From(handle);
            if (node is not null)
            {
                return new PlistDocument(node, PlistDocumentFormats.XML);
            }
            return null;
        }

        /// <summary>
        /// Get a plist document from <see cref="byte"/> array
        /// </summary>
        /// <param name="bin"></param>
        /// <returns></returns>
        public static PlistDocument? FromBin(byte[] bin)
        {
            uint length = (uint)bin.Length;
            plist_from_bin(bin, length, out var handle);
            var node = PlistNode.From(handle);
            if (node is not null)
            {
                return new PlistDocument(node, PlistDocumentFormats.Binary);
            }
            return null;
        }

        /// <summary>
        /// Get a plist document from <see cref="byte"/> array
        /// </summary>
        /// <param name="bin"></param>
        /// <returns></returns>
        public static PlistDocument? FromBin(ReadOnlySpan<byte> bin)
        {
            uint length = (uint)bin.Length;
            Native.PlistHandle handle;
#if NET7_0_OR_GREATER
                    plist_from_bin(bin, length, out handle);
#else
            unsafe
            {
                fixed (byte* ptr = bin)
                {
                    plist_from_bin(ptr, length, out handle);
                }
            }
#endif
            var node = PlistNode.From(handle);
            if (node is not null)
            {
                return new PlistDocument(node, PlistDocumentFormats.Binary);
            }
            return null;
        }

        /// <summary>
        /// Get plist data from stream.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <returns>The plist Document</returns>

        public static PlistDocument? Load(Stream stream)
        {
            return stream switch {
                UnmanagedMemoryStream ums => LoadInternal(ums),
                MemoryStream ms => LoadInternal(ms),
                _ => LoadInternal(stream)
            };
        }

        private static PlistDocument? LoadInternal(Stream stream)
        {
            PlistDocument? document = null;

            uint leight = (uint)stream.Length;
            var ptr = IntPtr.Zero;
            try
            {
#if NET6_0_OR_GREATER
                unsafe
                {
                    ptr = (IntPtr)NativeMemory.Alloc(leight);
                }
#else
                ptr = Marshal.AllocHGlobal((nint)leight);
#endif
                unsafe
                {
                    using var ums = new UnmanagedMemoryStream((byte*)ptr, stream.Length, stream.Length, FileAccess.Write);
                    stream.CopyTo(ums);

                    plist_from_memory((byte*)ptr, leight, out var handle);
                    var node = PlistNode.From(handle);
                    if (node is not null)
                    {
                        var format = plist_is_binary((byte*)ptr, leight) != 0 ? PlistDocumentFormats.Binary : PlistDocumentFormats.XML;
                        document = new PlistDocument(node, format);
                    }
                }
            }
            finally
            {
#if NET6_0_OR_GREATER
                unsafe
                {
                    NativeMemory.Free((byte*)ptr);
                }
#else
                Marshal.FreeHGlobal(ptr);
#endif
            }
            return document;
        }

        private static PlistDocument? LoadInternal(MemoryStream stream)
        {
            if (stream.TryGetBuffer(out var buffer))
            {

                var length = (uint)buffer.Count;
#if NET7_0_OR_GREATER
                plist_from_memory(buffer, length, out var handle);
#else
                var ArrayOffset = new ArrayWithOffset(buffer.Array, buffer.Offset);
                plist_from_memory(ArrayOffset, length, out var handle);
#endif
                stream.Seek(length, SeekOrigin.Current);
                var node = PlistNode.From(handle);
                if (node is not null)
                {
#if NET7_0_OR_GREATER
                    var format = plist_is_binary(buffer, length) != 0 ? PlistDocumentFormats.Binary : PlistDocumentFormats.XML;
#else
                    var format = plist_is_binary(ArrayOffset, length) != 0 ? PlistDocumentFormats.Binary : PlistDocumentFormats.XML;
#endif
                    return new PlistDocument(node, format);
                }
            }
            else
            {
                return LoadInternal((Stream)stream);
            }
            return null;
        }

        private static unsafe PlistDocument? LoadInternal(UnmanagedMemoryStream stream)
        {
            var ptr = stream.PositionPointer;
            var length = (uint)(stream.Length - stream.Position);
            plist_from_memory(ptr, length, out var handle);
            stream.Seek(length, SeekOrigin.Current);
            var node = PlistNode.From(handle);
            if (node is not null)
            {
                var format = plist_is_binary(ptr, length) != 0 ? PlistDocumentFormats.Binary : PlistDocumentFormats.XML;
                return new PlistDocument(node, format);
            }
            return null;
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        /// <summary>
        /// Get plist data from stream.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <param name="token">The <see cref="CancellationToken"/>for the task</param>
        /// <returns>The plist Document</returns>
        public static async Task<PlistDocument?> LoadAsync(Stream stream, CancellationToken token)
        {
            return stream switch
            {
                UnmanagedMemoryStream ums => await LoadInternalAsync(ums, token),
                MemoryStream ms => await LoadInternalAsync(ms, token),
                _ => await LoadInternalAsync(stream, token),
            };
        }
#else
        /// <summary>
        /// Get plist data from stream.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <returns>The plist Document</returns>
        public static async Task<PlistDocument?> LoadAsync(Stream stream)
        {
            return stream switch
            {
                UnmanagedMemoryStream ums => await LoadInternalAsync(ums),
                MemoryStream ms => await LoadInternalAsync(ms),
                _ => await LoadInternalAsync(stream),
            };
        }
#endif


#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        /// <summary>
        /// Get plist data from stream.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <param name="token">The <see cref="CancellationToken"/>for the task</param>
        /// <returns>The plist Document</returns>
        private static async Task<PlistDocument?> LoadInternalAsync(Stream stream, CancellationToken token)
#else
        /// <summary>
        /// Get plist data from stream.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <returns>The plist Document</returns>
        private static async Task<PlistDocument?> LoadInternalAsync(Stream stream)
#endif
        {
            PlistDocument? document = null;

            uint leight = (uint)stream.Length;
            var safeptr = IntPtr.Zero;
            UnmanagedMemoryStream? ums = null;
            try
            {

#if NET6_0_OR_GREATER
                unsafe
                {
                    safeptr = (IntPtr)NativeMemory.Alloc(leight);

                }
#else
                    safeptr = Marshal.AllocHGlobal((nint)leight);
#endif
                unsafe
                {
                    ums = new UnmanagedMemoryStream((byte*)safeptr, stream.Length, stream.Length, FileAccess.Write);
                }
                using(ums)
                {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER

                    await stream.CopyToAsync(ums, token);
#else
                    await stream.CopyToAsync(ums);
#endif
                }
                unsafe
                {
                    var ptr = (byte*)safeptr;
                    plist_from_memory(ptr, leight, out var handle);
                    var node = PlistNode.From(handle);
                    if (node is not null)
                    {
                        var format = plist_is_binary(ptr, leight) != 0 ? PlistDocumentFormats.Binary : PlistDocumentFormats.XML;
                        document = new PlistDocument(node, format);
                    }
                }
            }
            finally
            {
                if (safeptr != IntPtr.Zero)
                {
#if NET6_0_OR_GREATER
                    unsafe
	                {
                        NativeMemory.Free((byte*)safeptr);
	                }
#else
                    Marshal.FreeHGlobal(safeptr);
#endif
                }
            }
            return document;
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        private static Task<PlistDocument?> LoadInternalAsync(UnmanagedMemoryStream stream, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return Task.FromCanceled<PlistDocument?>(token);
            }
#else
        private static Task<PlistDocument?> LoadInternalAsync(UnmanagedMemoryStream stream)
        {
#endif

            return Task.FromResult(LoadInternal(stream));
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        private static Task<PlistDocument?> LoadInternalAsync(MemoryStream stream, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return Task.FromCanceled<PlistDocument?>(token);
            }
#else
        private static Task<PlistDocument?> LoadInternalAsync(MemoryStream stream)
        {
#endif
            return Task.FromResult(LoadInternal(stream));
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        /// <summary>
        /// Get plist data from stream.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <returns>The plist Document</returns>
        public static async Task<PlistDocument?> LoadAsync(Stream stream)
        {
            return await LoadAsync(stream, CancellationToken.None);
        }
#endif

        private delegate void plist_to_data(Native.PlistHandle plist, out IntPtr plistXml, out uint length);

        private delegate void plist_to_data_free(IntPtr plistXml);

        /// <summary>
        /// Save a Plist document on the specified <paramref name="stream"/> with the specified <paramref name="format"/>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="format"></param>
        /// <exception cref="FormatException"></exception>
        public void SaveAs(Stream stream, PlistDocumentFormats format)
        {
            (plist_to_data toData, plist_to_data_free freeData) delegates = format switch
            {
                PlistDocumentFormats.XML => (plist_to_xml, plist_to_xml_free),
                PlistDocumentFormats.Binary => (plist_to_bin, plist_to_bin_free),
                _ => throw new FormatException()
            };
            var ptr = IntPtr.Zero;
            try
            {
                delegates.toData(RootNode.Handle, out ptr, out var length);
                unsafe
                {
                    using var ums = new UnmanagedMemoryStream((byte*)ptr.ToPointer(), length);
                    stream.Seek(0, SeekOrigin.Begin);
                    ums.CopyTo(stream);
                    stream.SetLength(ums.Length);
                    stream.Flush();
                }

            }
            finally
            {
                delegates.freeData(ptr);
            }

        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        /// <summary>
        /// Save a Plist document on the specified <paramref name="stream"/> with the specified <paramref name="format"/>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="format"></param>
        /// <param name="token">The cancellation token.</param>
        /// <exception cref="FormatException"></exception>
        public async Task SaveAsAsync(Stream stream, PlistDocumentFormats format, CancellationToken token)
#else
        /// <summary>
        /// Save a Plist document on the specified <paramref name="stream"/> with the specified <paramref name="format"/>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="format"></param>
        /// <exception cref="FormatException"></exception>
        public async Task SaveAsAsync(Stream stream, PlistDocumentFormats format)
#endif
        {
            (plist_to_data toData, plist_to_data_free freeData) delegates = format switch
            {
                PlistDocumentFormats.XML => (plist_to_xml, plist_to_xml_free),
                PlistDocumentFormats.Binary => (plist_to_bin, plist_to_bin_free),
                _ => throw new FormatException()
            };
            var ptr = IntPtr.Zero;
            try
            {
                delegates.toData(RootNode.Handle, out ptr, out var length);
                UnmanagedMemoryStream? ums = null;
                unsafe
                {
                    ums = new UnmanagedMemoryStream((byte*)ptr.ToPointer(), length);
                }
                using (ums)
                {
                    stream.Seek(0, SeekOrigin.Begin);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                    await ums.CopyToAsync(stream, token);
#else
                    await ums.CopyToAsync(stream);
#endif
                    stream.SetLength(ums.Length);
                    stream.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    delegates.freeData(ptr);
                }
            }
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        /// <summary>
        /// Save a Plist document on the specified <paramref name="stream"/> with the specified <paramref name="format"/>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="format"></param>
        /// <exception cref="FormatException"></exception>
        public async Task SaveAsAsync(Stream stream, PlistDocumentFormats format)
        {
            await SaveAsAsync(stream, format, CancellationToken.None);
        }
#endif

        /// <summary>
        /// Save the document on the specified <paramref name="stream"/> with the predefined format
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            SaveAs(stream, Format);
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        /// <summary>
        /// Save the document on the specified <paramref name="stream"/> with the predefined format
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="token">The cancellation token.</param>
        public async Task SaveAsync(Stream stream, CancellationToken token)
        {
            await SaveAsAsync(stream, Format, token);
        }
#endif

        /// <summary>
        /// Save the document on the specified <paramref name="stream"/> with the predefined format
        /// </summary>
        /// <param name="stream"></param>
        public async Task SaveAsync(Stream stream)
        {
            await SaveAsAsync(stream, Format);
        }

        public void Dispose() => ((IDisposable)RootNode).Dispose();
    }
}
