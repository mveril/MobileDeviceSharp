#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace MobileDeviceSharp.Native
{
#pragma warning disable SYSLIB1055 // Le type managé spécifié n’est pas valide
    [CustomMarshaller(typeof(IReadOnlyDictionary<string, string>), MarshalMode.Default, typeof(Utf8StringDictionaryMarshaller))]
#pragma warning restore SYSLIB1055 // Le type managé spécifié n’est pas valide
    public static class Utf8StringDictionaryMarshaller
    {
    }
}
#endif
