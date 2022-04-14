using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace iOSLib.SourceGenerator
{
    internal class ExceptionSyntaxRecever : ISyntaxContextReceiver, ISourceInfoContainer
    {
        private readonly List<ExceptionInfo> _exceptionInfos = new();
        public IEnumerable<SourceCodeInfoBase> SourceCodeInfos => _exceptionInfos.Cast<SourceCodeInfoBase>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            var @enum = context.Node as EnumDeclarationSyntax;
            if (@enum != null)
            {
                if (@enum.Identifier.ToString().EndsWith("Error"))
                {
                    var enumSymbol = context.SemanticModel.GetDeclaredSymbol(@enum);
                    if (enumSymbol != null)
                        _exceptionInfos.Add(new ExceptionInfo(context.SemanticModel.Compilation, enumSymbol));
                }
            }
        }
    }
}
