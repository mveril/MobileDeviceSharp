using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace iOSLib.SourceGenerator
{
    [Generator]
    class ExceptionGenerator : IIncrementalGenerator
    {
        const string ExceptionBaseNamespace = "IOSLib";
        const string ExceptionBaseName = "MobileDeviceException";
        const string ExceptionBaseFullName = $"{ExceptionBaseNamespace}.{ExceptionBaseName}";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var source = context.SyntaxProvider.CreateSyntaxProvider(Predicate, Transform).Where((@enum)=> @enum != null);
            context.RegisterSourceOutput(source, Producer);
        }

        private bool Predicate(SyntaxNode node, CancellationToken token) => node.IsKind(SyntaxKind.EnumDeclaration) && ((EnumDeclarationSyntax)node).Identifier.ToString().EndsWith("Error");
        private ITypeSymbol Transform(GeneratorSyntaxContext context, CancellationToken token)
        {
            return (ITypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node);
        }

        private static void Producer(SourceProductionContext context, ITypeSymbol EnumSymbol)
        {
            var enumName = EnumSymbol.Name;
            var enumNamespaceName = EnumSymbol.ContainingNamespace.ToDisplayString();
            var parentNamespaceName = EnumSymbol.ContainingNamespace.ContainingNamespace.ToDisplayString();
            var count = "Error".Length;
            var enumNameWithoutError = enumName.Substring(0, enumName.Length - count);
            var exceptionName = $"{enumNameWithoutError}{nameof(Exception)}";
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
            context.AddSource($"{enumName}Extension.g.cs", sourceExtension);
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
}}", enumNamespaceName, parentNamespaceName, exceptionName, ExceptionBaseNamespace, enumName, ExceptionBaseName);
            context.AddSource($"{exceptionName}.g.cs", sourceException);
        }
    }
}
