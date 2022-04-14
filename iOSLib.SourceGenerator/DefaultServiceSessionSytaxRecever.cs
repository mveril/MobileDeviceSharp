using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace iOSLib.SourceGenerator
{
    internal class DefaultServiceSessionSytaxRecever : ISyntaxContextReceiver, ISourceInfoContainer
    {
        readonly List<DefaultServiceSessionInfo> _defaultServiceSessionInfos = new();
        public DefaultServiceSessionSytaxRecever()
        {

        }

        public IEnumerable<SourceCodeInfoBase> SourceCodeInfos => _defaultServiceSessionInfos.Cast<SourceCodeInfoBase>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax type)
            {
                var serviceSessionBase = context.SemanticModel.Compilation.GetTypeByMetadataName("IOSLib.ServiceSessionBase`2");
                if (type.Identifier.ToString().EndsWith("Base"))
                {
                    var typeSymbol = context.SemanticModel.GetDeclaredSymbol(type);
                    if (typeSymbol != null)
                    {
                        var bt = typeSymbol.BaseType;
                        var btbt = bt?.OriginalDefinition;
                        if ((btbt?.IsGenericType).GetValueOrDefault(false))
                        {
                            if (btbt!.OriginalDefinition.Equals(serviceSessionBase, SymbolEqualityComparer.Default))
                            {
                                _defaultServiceSessionInfos.Add(new DefaultServiceSessionInfo(typeSymbol));
                            }
                        }
                    }
                }
            }
        }
    }
}
