using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace iOSLib.SourceGenerator
{
    internal class NotificationProxyEventMappingSyntaxRecever : ISyntaxContextReceiver, ISourceInfoContainer
    {
        internal const string AttrNamespace = "IOSLib.CompilerServices";
        internal const string AttrName = "NotificationProxyEventNameAttribute";
        internal const string PropName = "Name";
        private readonly List<NotificationProxyEventMappingInfo> _NPMapingInfos = new();

        public IEnumerable<SourceCodeInfoBase> SourceCodeInfos => _NPMapingInfos.Cast<SourceCodeInfoBase>();


        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax typeDeclaration)
            {
                if (typeDeclaration.Kind() == SyntaxKind.ClassDeclaration)
                {
                    var target = context.SemanticModel.Compilation.GetTypeByMetadataName("IOSLib.IDevice");
                    var declaredsymbol = context.SemanticModel.GetDeclaredSymbol(typeDeclaration);
                    if (declaredsymbol != null)
                    {
                        INamedTypeSymbol typeSymbol = (INamedTypeSymbol)declaredsymbol;
                        var npNameAttrSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName($"{AttrNamespace}.{AttrName}");
                        if (typeSymbol.GetMembers().Any(m => m.Kind == SymbolKind.Event && ((IEventSymbol)m).GetAttributes().Any(a => a.AttributeClass.Equals(npNameAttrSymbol, SymbolEqualityComparer.Default))))
                        {
                            _NPMapingInfos.Add(new NotificationProxyEventMappingInfo(typeSymbol, context.SemanticModel.Compilation));
                        }
                    }
                }
            }
        }
    }
}
