using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace iOSLib.SourceGenerator
{
    internal abstract class SourceCodeInfoBase
    {
        internal abstract IReadOnlyDictionary<string, string> BuildSource();
    }
}
