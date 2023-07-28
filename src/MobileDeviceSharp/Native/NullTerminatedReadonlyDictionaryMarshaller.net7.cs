#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace MobileDeviceSharp.Native
{

    [CustomMarshaller(typeof(ReadOnlyStringDictionary), MarshalMode.Default, typeof(ReadOnlyStringDictionaryMarshaller))]
    public static unsafe class ReadOnlyStringDictionaryMarshaller
    {
        public static byte** ConvertToUnmanaged(ReadOnlyStringDictionary managed)
        {
            var ptr = (byte**)Marshal.AllocCoTaskMem(managed.Count * IntPtr.Size).ToPointer();
            var span = MarshalUtils.GetNullTerminatedSpan((IntPtr*)ptr);
            var dicList = (IReadOnlyCollection<KeyValuePair<string, string?>>)managed;
            int index = -1;
            foreach ( var kvp in dicList )
            {
                span[index++] = (IntPtr)Utf8StringMarshaller.ConvertToUnmanaged(kvp.Key);
                span[index++] = (IntPtr)Utf8StringMarshaller.ConvertToUnmanaged(kvp.Value);
            }
            return ptr;
        }

        public static ReadOnlyStringDictionary ConvertToManaged(byte** unmanaged)
        {
            var dic = new Dictionary<string, string?>();
            var unmanagedArray = MarshalUtils.GetNullTerminatedReadOnlySpan((IntPtr*)unmanaged);
            if (int.IsOddInteger(unmanagedArray.Length))
            {
                throw new NotSupportedException();
            }
            var enumerator = unmanagedArray.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var key = Utf8StringMarshaller.ConvertToManaged((byte*)enumerator.Current);
                if (key is null)
                {
                    throw new NullReferenceException();
                }
                if (!enumerator.MoveNext())
                {
                    throw new NotSupportedException();
                }
                var value = Utf8StringMarshaller.ConvertToManaged((byte*)enumerator.Current);
                dic.Add(key, value);
            }
            return new ReadOnlyStringDictionary(dic);
        }

        public static void Free(byte** unmanaged)
        {
            var ptr = unmanaged;
            while (*ptr != null)
            {
                Utf8StringMarshaller.Free(*ptr);
                ptr++;
            }
            Marshal.FreeCoTaskMem((IntPtr)ptr);
        }
    }
}
#endif
