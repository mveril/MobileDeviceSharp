#if NET7_0_OR_GREATER
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.Marshalling;

namespace MobileDeviceSharp.Native
{
    [NativeMarshalling(typeof(NullTerminatedReadonlyDictionaryMarshaller<,>))]
    public class SameKeyValueTypeReadonlyDictionary<T> : ReadOnlyDictionary<T, T?> where T : notnull
    {
        public SameKeyValueTypeReadonlyDictionary(IDictionary<T, T?> dictionary) : base(dictionary)
        {
        }
    }
}
#endif
