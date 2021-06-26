using IOSLib.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace iOSLib.SourceGenerator
{
    internal class ExceptionInfo : SourceCodeInfoBase
    {
        private readonly INamedTypeSymbol baseType;

        internal ExceptionInfo(Compilation compilation, ITypeSymbol enumSymbol)
        {
            if (enumSymbol.TypeKind != TypeKind.Enum)
            {
                throw new ArgumentException($"{enumSymbol.Name} must be an enum");
            }
            EnumSymbol = enumSymbol;
            baseType = compilation.GetTypeByMetadataName("IOSLib.MobileDeviceException");
        }
        internal ITypeSymbol EnumSymbol { get; set; }

        override internal IReadOnlyDictionary<string,string> BuildSource()
        {
            var dic = new Dictionary<string, string>();
            var enumName = EnumSymbol.Name;
            var enumNamespaceName = EnumSymbol.ContainingNamespace.ToDisplayString();
            var parentNamespaceName = EnumSymbol.ContainingNamespace.ContainingNamespace.ToDisplayString();
            var count = "Error".Length;
            var enumNameWithoutError = enumName.Substring(0, enumName.Length - count);
            var exceptionName = $"{enumNameWithoutError}{nameof(Exception)}";
            var baseName = baseType.ToDisplayString(new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes));
            var baseNameSpaceName = baseType.ContainingNamespace.ToDisplayString();
            var sourceExtension = string.Format(@"namespace {0}
{{
    public static class {1}Extension
    {{
        public static bool IsError(this {1} error)
        {{
            return error != {1}.Success;
        }}

        public static {2} GetException(this {1} error)
        {{
            if(error.IsError())
            {{
                return new {2}(error);
            }}
            return null;
        }}
        public static {2} GetException(this {1} error,string message)
        {{
            if(error.IsError())
            {{
                return new {2}(message, error);
            }}
            return null;
        }}
    }}
}}", EnumSymbol.ContainingNamespace.ToDisplayString(), enumName, exceptionName);
            dic.Add($"{enumName}Extension.g.cs",sourceExtension);
            var sourceException = string.Format(@"using {3};
using {0};

namespace {1}
{{
    public class {2} : {5}
    {{
        public {2}() : base()
        {{

        }}

        public {2}({4} errorCode) : base(GetMessageForHResult(errorCode),(int)errorCode)
        {{

        }}

        public {2}(string message, {4} errorCode) : base(message, (int)errorCode)
        {{

        }}
    }}
}}", enumNamespaceName, parentNamespaceName, exceptionName,baseNameSpaceName, enumName,baseName);
            dic.Add($"{exceptionName}.g.cs", sourceException);
            return dic;
        }
    }
}