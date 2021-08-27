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
    class NotificationProxyEventMappingGenerator : ISourceGenerator
    {
        internal const string AttrNamespace = "IOSLib.CompilerServices";
        internal const string AttrName = "NotificationProxyEventNameAttribute";
        internal const string PropName = "Name";

        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            IEnumerable<SyntaxNode> allNodes = compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes());

            IEnumerable<ClassDeclarationSyntax> allClass = allNodes
                .Where(d => d.IsKind(SyntaxKind.ClassDeclaration))
                .OfType<ClassDeclarationSyntax>();
            foreach (var type in allClass)
            {
                var info = TryGetNPMapingInfo(compilation, type);
                if (info != null)
                {
                    info.AddTo(context);
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
        }

        internal NotificationProxyEventMappingInfo? TryGetNPMapingInfo(Compilation compilation, TypeDeclarationSyntax typeDeclaration)
        {
            if (typeDeclaration.Kind() == Microsoft.CodeAnalysis.CSharp.SyntaxKind.ClassDeclaration)
            {
                var semanticModel = compilation.GetSemanticModel(typeDeclaration.SyntaxTree);
                var target = compilation.GetTypeByMetadataName("IOSLib.IDevice");
                INamedTypeSymbol typeSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(typeDeclaration);
                var npNameAttrSymbol = compilation.GetTypeByMetadataName($"{AttrNamespace}.{AttrName}");
                if (typeSymbol.GetMembers().Any(m => m.Kind == SymbolKind.Event && ((IEventSymbol)m).GetAttributes().Any(a => a.AttributeClass.Equals(npNameAttrSymbol, SymbolEqualityComparer.Default))))
                {
                    return new NotificationProxyEventMappingInfo(typeSymbol, compilation);
                }
            }            
            return null;
        }

    }
}
