using iOSLib.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace iOSLib.SourceGenerator
{
    internal static class GeneratorExecutionContextExtension
    {
        public static Compilation AddSourceAndGetCompilation(this GeneratorExecutionContext context, string name, SourceText text)
        {
            context.AddSource(name, text);
            return context.Compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(text, (CSharpParseOptions)context.ParseOptions));
        }

        public static void AddSource(this GeneratorExecutionContext context, SourceCodeInfoBase infoBase)
        {
            foreach (var item in infoBase.BuildSource())
            {
                context.AddSource($"{item.Key}.g.cs", item.Value);
            }
        }
    }
}
