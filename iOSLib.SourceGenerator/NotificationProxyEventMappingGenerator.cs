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

        public void Execute(GeneratorExecutionContext context)
        {
            var recever = context.SyntaxContextReceiver as NotificationProxyEventMappingSyntaxRecever;
            if (recever != null)
            {
                foreach (var info in recever.SourceCodeInfos)
                {
                    context.AddSource(info);
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(()=> new NotificationProxyEventMappingSyntaxRecever());
        }
    }
}
