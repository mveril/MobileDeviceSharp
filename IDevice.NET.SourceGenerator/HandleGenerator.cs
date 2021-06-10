using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDevice.NET.SourceGenerator
{
    [Generator]
    public class HandleGenerator : ISourceGenerator
    {
        const string AttrNamespace = "IDevice.NET.Generator";
        const string AttrName = "GenerateHandleAttribute";
        const string PropName = "HandleName";
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            string attrSource = GetAttrSorce();
            context.AddSource("GenerateHandleAttribute.g.cs", attrSource);
            IEnumerable<SyntaxNode> allNodes = compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes());

            IEnumerable<MethodDeclarationSyntax> allMethods = allNodes
                .Where(d => d.IsKind(SyntaxKind.MethodDeclaration))
                .OfType<MethodDeclarationSyntax>();
            foreach (var method in allMethods)
            {
                var info = TryGetHandleInfo(compilation,method);
                if (info!= null)
                {
                    var source = info.BuildSource();
                    if (source != null)
                    {
                        context.AddSource($"{info.HandleName}.g.cs", source);
                    }
                }
            }
        }


        private string GetAttrSorce()
        {
            var argName = (string)(PropName[0].ToString()).ToLower() + PropName.Substring(1);
            return string.Format(@"using System;

namespace {0}
{{
    [AttributeUsage(AttributeTargets.Method)]
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
}}", AttrNamespace, AttrName, PropName, argName);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            Debugger.Launch();
        }

        /*internal MethodDeclarationSyntax GetHandleMethod(Compilation compilation, ClassDeclarationSyntax classDeclaration)
        {

            var methods = classDeclaration.Members.OfType<MethodDeclarationSyntax>();
            var method = methods.FirstOrDefault(m => CheckIsFreeEntryPointOf(compilation, m, classDeclaration.Identifier.ToString()));
            return method;
        }*/
        internal HandleInfo TryGetHandleInfo(Compilation compilation, MethodDeclarationSyntax methodDeclaration)
        {
            var fullname =  $"{AttrNamespace}.{AttrName}";
            var attributes = methodDeclaration.AttributeLists
    .SelectMany(x => x.Attributes);
            var fln = attributes.Select(a => a.Name.ToFullString());
            var genAttr = attributes.FirstOrDefault(attr => attr.Name.ToFullString() == fullname);
            if (genAttr==null)
            {
                return null;
            }
            var semanticModel = compilation.GetSemanticModel(methodDeclaration.SyntaxTree);
            var genArg = genAttr.ArgumentList.Arguments[0];
            var genExpr = genArg.Expression;
            var genName = semanticModel.GetConstantValue(genExpr).ToString();
            var info = new HandleInfo(methodDeclaration, genName);
            return info;
        }
    }
}
