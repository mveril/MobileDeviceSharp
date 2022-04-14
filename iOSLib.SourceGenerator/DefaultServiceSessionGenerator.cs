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
            if (context.SyntaxContextReceiver is DefaultServiceSessionSytaxRecever recever)
            {
                foreach (var info in recever.SourceCodeInfos)
                {
                    context.AddSource(info);
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new DefaultServiceSessionSytaxRecever());
        }
    }
}
