using IOSLib.SourceGenerator;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace iOSLib.SourceGenerator
{
    internal class DefaultServiceSessionInfo : SourceCodeInfoBase
    {
        public  INamedTypeSymbol TypeSymbol { get; }

        public DefaultServiceSessionInfo(INamedTypeSymbol typeSymbol)
        {
            TypeSymbol = typeSymbol;
        }

        internal override IReadOnlyDictionary<string, string> BuildSource()
        {
            string nameSpaceName = TypeSymbol.ContainingNamespace.ToDisplayString();
            string className = TypeSymbol.Name.Remove(TypeSymbol.Name.Length - 4, 4);
            var sourceFormat = @"namespace {0}
{{
    public class {1} : {1}Base
    {{
        public {1}(IDevice device) : base(device) {{ }}
    }}
}}";
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>() { { $"{className}.g.cs", string.Format(sourceFormat, nameSpaceName, className) } });
        }
    }
}