using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    /// <summary>
    /// Represent a Property list document
    /// </summary>
    public class PlistDocument : IDisposable
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
        public PlistDocumentFormats Format { get; set; }


        /// <summary>
        /// Get XML <see cref="string"/> from the plist.
        /// </summary>
        /// <returns>XML <see cref="string"/></returns>
        public string ToXMLString()
        {
            plist_to_xml(RootNode.Handle, out var xmlptr, out var leight);
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
            plist_to_bin(RootNode.Handle, out IntPtr ptr, out var length);
            byte[] buffer = new byte[length];
            Marshal.Copy(ptr, buffer, 0, (int)length);
            plist_to_bin_free(ptr);
            return buffer;
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
        /// Get a plist document from <see cref="byte"/> array
        /// </summary>
        /// <param name="bin"></param>
        /// <returns></returns>
        public static PlistDocument? FromBin(byte[] bin)
        {
            uint length = (uint)bin.Length;
            plist_from_bin(bin, length, out var handle);
            using var node = PlistNode.From(handle);
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
            PlistDocument? document = null;

            uint leight = (uint)stream.Length;
            unsafe
            {
                byte* ptr;
#if NET6_0_OR_GREATER
                ptr = (byte*)NativeMemory.Alloc(leight);
#else
                ptr = (byte*)Marshal.AllocHGlobal((nint)leight).ToPointer();
#endif
                var ums = new UnmanagedMemoryStream(ptr, stream.Length, stream.Length, FileAccess.Write);
                stream.CopyTo(ums);
                stream.Close();
                plist_from_memory(ptr, leight, out var handle);
                var node = PlistNode.From(handle);
                if (node is not null)
                {
                   var format = plist_is_binary(ptr, leight) != 0 ? PlistDocumentFormats.Binary : PlistDocumentFormats.XML;
                   document = new PlistDocument(node, format);
                }

#if NET6_0_OR_GREATER
                NativeMemory.Free(ptr);
#else
                Marshal.FreeHGlobal((IntPtr)ptr);
#endif             
            }
            return document;
        }


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
            delegates.toData(RootNode.Handle, out var ptr, out var length);
            unsafe
            {
                var ums = new UnmanagedMemoryStream((byte*)ptr.ToPointer(), length);
                stream.Seek(0, SeekOrigin.Begin);
                ums.CopyTo(stream);
                stream.SetLength(ums.Length);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                ums.Close();
            }
            delegates.freeData(ptr);
        }

        /// <summary>
        /// Save the document on the specified <paramref name="stream"/> with the predefined format
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            SaveAs(stream, Format);
        }

        public void Dispose() => ((IDisposable)RootNode).Dispose();
    }
}
