using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.IO;
using System;

namespace iOSLib.SourceGenerator
{
    [Generator]
    internal class HandleGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            
            if (context.SyntaxContextReceiver is HandleSyntaxRecever handleSyntaxRecever)
            {
                foreach (var info in handleSyntaxRecever.SourceCodeInfos)
                {
                    context.AddSource(info);
                }
            }
            var file = context.AdditionalFiles.FirstOrDefault(p => Path.GetFileName(p.Path).Equals("GenrateHandle.txt", StringComparison.OrdinalIgnoreCase));
            if (file != null)
            {
                var text = file.GetText();
                if (text != null)
                {
                    foreach (var line in text.Lines)
                    {
                        try
                        {
                            var span = line.Span;
                            if (!span.IsEmpty)
                            {
                                var info = new NonFreeableHandleInfo(text.ToString(span));
                                foreach (var source in info.BuildSource())
                                {
                                    context.AddSource(source.Key, source.Value);
                                }
                            }   
                        }
                        catch (Exception ex)
                        {

                            Debug.WriteLine(ex);
                        }
                    }
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
            context.RegisterForSyntaxNotifications(() => new HandleSyntaxRecever());
        }
    }
}
