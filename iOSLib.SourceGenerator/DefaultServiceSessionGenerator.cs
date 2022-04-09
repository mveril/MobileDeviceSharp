using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace iOSLib.SourceGenerator
{
    [Generator]
    class DefaultServiceSessionGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            IEnumerable<SyntaxNode> allNodes = compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes());

            IEnumerable<ClassDeclarationSyntax> allClass = allNodes
                .Where(d => d.IsKind(SyntaxKind.ClassDeclaration))
                .OfType<ClassDeclarationSyntax>();
            foreach (var type in allClass)
            {
                    var info = TryGetServiceBaseInfo(compilation, type);
                    if (info != null)
                    {
                        info.AddTo(context);
                    }
            }
        }

        private DefaultServiceSessionInfo? TryGetServiceBaseInfo(Compilation compilation, ClassDeclarationSyntax type)
        {
            var serviceSessionBase = compilation.GetTypeByMetadataName("IOSLib.ServiceSessionBase`2");
            if (type.Identifier.ToString().EndsWith("Base"))
            {
                var typeSymbol = compilation.GetSemanticModel(type.SyntaxTree).GetDeclaredSymbol(type);
                if (typeSymbol != null)
                {
                    var bt = typeSymbol.BaseType;
                    var btbt = bt?.OriginalDefinition;
                    if ((btbt?.IsGenericType).GetValueOrDefault(false))
                    {
                        if (btbt!.OriginalDefinition.Equals(serviceSessionBase, SymbolEqualityComparer.Default))
                        {
                            return new DefaultServiceSessionInfo(typeSymbol);
                        }
                    }
                }
            }
            return null;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            
        }
    }
}
