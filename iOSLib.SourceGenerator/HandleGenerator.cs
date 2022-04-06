using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.IO;
using System;

namespace IOSLib.SourceGenerator
{
    [Generator]
    internal class HandleGenerator : ISourceGenerator
    {
        const string AttrNamespace = "IOSLib.CompilerServices";
        const string AttrName = "GenerateHandleAttribute";
        const string PropName = "HandleName";
        public void Execute(GeneratorExecutionContext context)
        {
            var attrSource = SourceText.From(GetAttrSorce(), Encoding.UTF8);
            var compilation = context.Compilation;
            IEnumerable<SyntaxNode> allNodes = compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes());

            IEnumerable<MethodDeclarationSyntax> allMethods = allNodes
                .Where(d => d.IsKind(SyntaxKind.MethodDeclaration))
                .OfType<MethodDeclarationSyntax>();
            foreach (var method in allMethods)
            {
                try
                {
                    var info = TryGetHandleInfo(compilation, method);
                    if (info != null)
                    {
                        var source = info.BuildSource();
                        if (source != null)
                        {
                            info.AddTo(context);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            var file = context.AdditionalFiles.FirstOrDefault(p => Path.GetFileName(p.Path).Equals("GenrateHandle.txt", StringComparison.OrdinalIgnoreCase));
            if (file != null)
            {
                var text = file.GetText();
                foreach (var line in text.Lines)
                {
                    var span = line.Span;
                    if (!span.IsEmpty)
                    {
                        var info = new HandleInfo(text.ToString(span));
                        info.AddTo(context);
                    }
                }
            }
        }


        private string GetAttrSorce()
        {
            var argName = (string)(PropName[0].ToString()).ToLower() + PropName.Substring(1);
            return string.Format(@"using System;
using IOSLib.;

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
        internal HandleInfo TryGetHandleInfo(Compilation compilation, MethodDeclarationSyntax methodDeclaration)
        {
            var genAttrSymbol = compilation.GetTypeByMetadataName($"{AttrNamespace}.{AttrName}");
            var semanticModel = compilation.GetSemanticModel(methodDeclaration.SyntaxTree);
            var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);
            var genAttr = methodSymbol.GetAttributes().FirstOrDefault((attr) => attr.AttributeClass.Equals(genAttrSymbol, SymbolEqualityComparer.Default));
            if (genAttr==null)
            {
                return null;
            }
            var genName = (string)genAttr.ConstructorArguments[0].Value;
            var info = new HandleInfo(methodSymbol, genName, compilation);
            return info;
        }
    }
}
