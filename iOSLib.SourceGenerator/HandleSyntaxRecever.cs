using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace iOSLib.SourceGenerator
{
    internal class HandleSyntaxRecever : ISyntaxContextReceiver, ISourceInfoContainer
    {
        const string AttrNamespace = "IOSLib.CompilerServices";
        const string AttrName = "GenerateHandleAttribute";
        const string PropName = "HandleName";

        public IEnumerable<SourceCodeInfoBase> SourceCodeInfos => _handleInfos.Cast<SourceCodeInfoBase>();

        private List<HandleInfoBase> _handleInfos = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            var methodDeclaration = context.Node as MethodDeclarationSyntax;
            if (methodDeclaration != null)
            {
                var genAttrSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName($"{AttrNamespace}.{AttrName}");
                var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration) as IMethodSymbol;
                if (methodSymbol == null)
                {
                    return;
                }
                var genAttr = methodSymbol.GetAttributes().FirstOrDefault((attr) => attr.AttributeClass.Equals(genAttrSymbol, SymbolEqualityComparer.Default));
                if (genAttr == null)
                {
                    return;
                }
                var genName = (string)genAttr.ConstructorArguments[0].Value!;
                _handleInfos.Add(new FreeableHandleInfo(methodSymbol, genName, context.SemanticModel.Compilation));
            }
        }
    }
}
