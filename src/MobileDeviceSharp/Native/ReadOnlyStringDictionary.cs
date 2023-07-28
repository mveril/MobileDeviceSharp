#if NET7_0_OR_GREATER
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.Marshalling;

namespace MobileDeviceSharp.Native
{
    [NativeMarshalling(typeof(ReadOnlyStringDictionaryMarshaller))]
    public class ReadOnlyStringDictionary : ReadOnlyDictionary<string, string?>
    {
        public ReadOnlyStringDictionary(IDictionary<string, string?> dictionary) : base(dictionary)
        {
        }
    }
}
#endif
