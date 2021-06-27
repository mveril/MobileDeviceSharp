using IOSLib.SourceGenerator;
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

namespace iOSLib.SourceGenerator
{
    [Generator]
    class ExceptionGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            IEnumerable<SyntaxNode> allNodes = compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes());

            IEnumerable<EnumDeclarationSyntax> allEnum = allNodes
                .Where(d => d.IsKind(SyntaxKind.EnumDeclaration))
                .OfType<EnumDeclarationSyntax>();
            foreach (var @enum in allEnum)
            {
                var info = TryGetExceptionInfo(compilation, @enum);
                if (info != null)
                {
                    info.AddTo(context);
                }
            }
        }

        private ExceptionInfo TryGetExceptionInfo(Compilation compilation, EnumDeclarationSyntax @enum)
        {
            if (@enum.Identifier.ToString().EndsWith("Error"))
            {
                var enumSymbol = compilation.GetSemanticModel(@enum.SyntaxTree).GetDeclaredSymbol(@enum);
                return new ExceptionInfo(compilation, enumSymbol);
            }
            return null;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch(); 
        }

   }
}
