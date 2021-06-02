using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDevice.NET.SourceGenerator
{
    [Generator]
    public class HandleGenerator : ISourceGenerator
    {
        const string AttrNamespace = "iDevice.NET.Generator";
        const string AttrName = "GenerateHandleAttribute";
        const string PropName = "HandleName";
        public void Execute(GeneratorExecutionContext context)
        {
            string attrSource = GetAttrSorce();
            context.AddSource("GenerateHandleAttribute.g.cs", attrSource);
            /*var query = from typeSymbol in context.Compilation.SourceModule.GlobalNamespace.GetNamespaceTypes()
                        from method in typeSymbol.GetMethods()

                            // Find the attribute on the field
                        let info = method.FindAttributeFlattened(_generatedPropertyAttributeSymbol)
                        where info != null
                        select method;
            query*/


            /*
            IEnumerable<SyntaxNode> allNodes = context.Compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes());
            IEnumerable<ClassDeclarationSyntax> allClasses = allNodes
    .Where(d => d.IsKind(SyntaxKind.ClassDeclaration))
    .OfType<ClassDeclarationSyntax>();
            foreach (var classDeclaration in allClasses)
            {
                var method = GetHandleMethod(context.Compilation, classDeclaration);
                if (method != null)
                {
                    
                }
            }*/
        }

        private string GetAttrSorce()
        {
            var argName = (string)(PropName[0].ToString()).ToLower() + PropName.Substring(1);
            return string.Format(@"using System;

namespace {0}
{{
    [AttributeUsage(AttributeTargets.Method)]
    public class {1} : Attribute
    {{
        public string {2} {{ get; }}

        public {1}() : base()
        {{

        }}

        public {1}(string {3}) : base()
        {{
            {2} = {3};
        }}

    }}
}}", AttrNamespace, AttrName, PropName, argName);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
        }
        
        public MethodDeclarationSyntax GetHandleMethod(Compilation compilation, ClassDeclarationSyntax classDeclaration)
        {

            var methods = classDeclaration.Members.OfType<MethodDeclarationSyntax>();
            var method = methods.FirstOrDefault(m => CheckIsFreeEntryPointOf(compilation, m, classDeclaration.Identifier.ToString()));
            return method;
        }
        public bool CheckIsFreeEntryPointOf(Compilation compilation, MethodDeclarationSyntax methodDeclaration,string Name)
        {
            INamedTypeSymbol attributeSymbol = compilation.GetTypeByMetadataName($"System.Runtime.InteropServices.{nameof(DllImportAttribute)}");
            var model = compilation.GetSemanticModel(methodDeclaration.SyntaxTree);
            var methodSymbol= model.GetDeclaredSymbol(methodDeclaration);
            var attr = methodSymbol.GetAttributes().FirstOrDefault(ad => ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default));
            return attr.NamedArguments.ToImmutableDictionary()["EntryPoint"].Value.ToString() == $"{Name}_free";
        }
    }
}
