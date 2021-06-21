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
        const string AttrNamespace = "IOSLib.Generator.Exception";
        const string AttrGenName = "GenerateExceptionAttribute";
        const string AttrMessName = "ExceptionAttribute";
        const string PropMess = "Message";
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            foreach (var source in GetAttrsSorce())
            {
                var attrSource = SourceText.From(source.Value, Encoding.UTF8);
                compilation = context.AddSourceAndGetCompilation($"{source.Key}.g.cs", attrSource);
            }
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

        private IReadOnlyDictionary<string, string> GetAttrsSorce()
        {
            var dic = new Dictionary<string,string>();
            dic.Add(AttrGenName, string.Format(@"using System;

namespace {0}
{{
    [AttributeUsage(AttributeTargets.Enum)]
    public class {1} : Attribute
    {{

    }}
}}", AttrNamespace, AttrGenName));;;
            var argName = (string)(PropMess[0].ToString()).ToLower() + PropMess.Substring(1);
            dic.Add(AttrMessName, string.Format(@"using System;

namespace {0}
{{
    [AttributeUsage(AttributeTargets.Field)]
    public class {1} : Attribute
    {{
        public string {2} {{ get; }}

        public {1}() : base()
        {{

        }}

        public {1}(string {3}) : base()
        {{
            {2} = {3};
        }}

    }}
}}", AttrNamespace, AttrMessName,PropMess,argName));
            return dic;
        }
    }
}
