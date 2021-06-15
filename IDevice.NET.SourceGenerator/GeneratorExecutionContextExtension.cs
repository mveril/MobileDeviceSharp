using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDevice.NET.SourceGenerator
{
    internal static class GeneratorExecutionContextExtension
    {
        public static Compilation AddSourceAndGetCompilation(this GeneratorExecutionContext context, string name, SourceText text)
        {
            context.AddSource(name, text);
            return context.Compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(text, (CSharpParseOptions)context.ParseOptions));
        }
    }
}
