﻿using System.Globalization;

namespace MobileDeviceSharp.SourceGenerator
{
    [Generator]
    class ExceptionGenerator : IIncrementalGenerator
    {
        const string ExceptionBaseNamespace = "MobileDeviceSharp";
        const string ExceptionBaseName = "MobileDeviceException";
        const string ExceptionBaseFullName = $"{ExceptionBaseNamespace}.{ExceptionBaseName}";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var source = context.SyntaxProvider.CreateSyntaxProvider(Predicate, Transform).Where((@enum)=> @enum is not null);
            context.RegisterSourceOutput(source, Producer);
        }

        private bool Predicate(SyntaxNode node, CancellationToken token) => node.IsKind(SyntaxKind.EnumDeclaration) && ((EnumDeclarationSyntax)node).Identifier.ToString().EndsWith("Error");
        private ITypeSymbol Transform(GeneratorSyntaxContext context, CancellationToken token)
        {
            return (ITypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node)!;
        }

        private static readonly DiagnosticDescriptor s_diagnosticDescriptorNeedSuccess = new DiagnosticDescriptor("ErrorNeedSuccess",
            "Error need a success field value with a value of 0",
            "Error need a success field value with a value of 0",
            typeof(ExceptionGenerator).FullName,
            DiagnosticSeverity.Error,
            true);

        private static readonly DiagnosticDescriptor s_diagnosticDescriptorSuccessMustBe0 = new DiagnosticDescriptor("ErrorSuccessMustBe0",
    "Error the sucess value need a value of 0",
    "Error the sucess value need a value of 0",
    typeof(ExceptionGenerator).FullName,
    DiagnosticSeverity.Error,
    true);

        private static void Producer(SourceProductionContext context, ITypeSymbol EnumSymbol)
        {
            var successArray = EnumSymbol.GetMembers("Success");
            if ((successArray.Length != 1 || successArray[0] is not IFieldSymbol field))
            {
                foreach (var location in EnumSymbol.Locations)
                {
                    context.ReportDiagnostic(Diagnostic.Create(s_diagnosticDescriptorNeedSuccess, location));
                }
            }
            else if (!field.HasConstantValue || field.ConstantValue is not IConvertible val || val.ToDouble(CultureInfo.InvariantCulture) != 0)
            {
                foreach (var location in field.Locations)
                {
                    context.ReportDiagnostic(Diagnostic.Create(s_diagnosticDescriptorSuccessMustBe0, location));
                }
            }
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
    public sealed partial class {2} : {5}
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
