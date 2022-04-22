namespace iOSLib.SourceGenerator
{
    [Generator]
    internal class HandleGenerator : IIncrementalGenerator
    {
        const string AttrNamespace = "IOSLib.CompilerServices";
        const string AttrName = "GenerateHandleAttribute";
        const string PropName = "HandleName";

        
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var nonFreeableFullNames = context.AdditionalTextsProvider
                .Where(file => Path.GetFileName(file.Path).Equals("GenrateHandle.txt", StringComparison.OrdinalIgnoreCase))
                .Select((file, token) => file.GetText())
                .Where((text) => text != null)
                .SelectMany((text, token) => text!.Lines)
                .Where((line) => !line.Span.IsEmpty)
                .Select((line, token) => line.Text!.ToString(line.Span));
            context.RegisterSourceOutput(nonFreeableFullNames, NonFreeableProducer);
            var freeableMethods = context.SyntaxProvider.CreateSyntaxProvider(MethodPredicate, MethodTransformer);
            context.RegisterSourceOutput<(string, string, string)?>(freeableMethods, FreableProducer);
        }

        private void FreableProducer(SourceProductionContext context, (string namespaceName, string className, string freeCode)? methodData)
        {
            if (methodData.HasValue)
            {
                var (nameSpace, className, freeCode) = methodData.Value;
                var (fileName,source) = GetSource(nameSpace, className, freeCode);
                context.AddSource(fileName, source);
            }
        }

        private void NonFreeableProducer(SourceProductionContext context, string fullClassName)
        {
            var index = fullClassName.LastIndexOf(".");
            var namespaceName = fullClassName.Substring(0, index);
            var handleBaseName = fullClassName.Substring(index + 1);
            var (fileName, source) = GetSource(namespaceName, handleBaseName,GetFreeCode());
            context.AddSource(fileName, source);
        }

        private (string name, SourceText source) GetSource(string namespaceName, string handleBaseName, string freeCode)
        {
            const string sourceFormat = @"using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace {0}
{{
    /// <summary>
    /// Represents a wrapper class for {1} handles.
    /// </summary>
#if !NETCOREAPP
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode=true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode=true)]
#endif
    public partial class {1} : IOSLib.Native.IOSHandle
    {{
        /// <summary>
        /// Initializes a new instance of the <see cref=""{1}""/> class.
        /// </summary>
        public {1}() : base()
        {{

        }}

        /// <summary>
        /// Gets a value which represents a pointer or handle that has been initialized to zero.
        /// </summary>
        public static {1} Zero
        {{
            get
            {{
                return new {1}();
            }}
        }}

        /// <inheritdoc/>
#if !NETCOREAPP
        [ReliabilityContractAttribute(Consistency.WillNotCorruptState, Cer.MayFail)]
#endif
        protected override bool ReleaseHandle()
        {{
{2}
        }}

        /// <inheritdoc/>
        public override string ToString()
        {{
            return $""{{this.handle}} ({{nameof({1})}})"";
        }}

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {{
            if (obj != null && obj.GetType() == typeof({1}))
            {{
                return (({1})obj).handle.Equals(this.handle);
            }}
            else
            {{
                return false;
            }}
        }}

        /// <inheritdoc/>
        public override int GetHashCode()
        {{
            return this.handle.GetHashCode();
        }}

        /// <summary>
        /// Determines whether two specified instances of <see cref=""{0}""/> are equal.
        /// </summary>
        /// <param name=""value1"">
        /// The first pointer or handle to compare.
        /// </param>
        /// <param name=""value2"">
        /// The second pointer or handle to compare.
        /// </param>
        /// <returns>
        /// <see langword=""true""/> if <paramref name=""value1""/> equals <paramref name=""value2""/>; otherwise, <see langword=""false""/>.
        /// </returns>
        public static bool operator ==({1} value1, {1} value2)
        {{
            if (object.Equals(value1, null) && object.Equals(value2, null))
            {{
                return true;
            }}

            if (object.Equals(value1, null) || object.Equals(value2, null))
            {{
                return false;
            }}

            return value1.handle == value2.handle;
        }}

        /// <summary>
        /// Determines whether two specified instances of <see cref=""{0}""/> are not equal.
        /// </summary>
        /// <param name=""value1"">
        /// The first pointer or handle to compare.
        /// </param>
        /// <param name=""value2"">
        /// The second pointer or handle to compare.
        /// </param>
        /// <returns>
        /// <see langword=""true""/> if <paramref name=""value1""/> does not equal <paramref name=""value2""/>; otherwise, <see langword=""false""/>.
        /// </returns>
        public static bool operator !=({1} value1, {1} value2)
        {{
            if (object.Equals(value1, null) && object.Equals(value2, null))
            {{
                return false;
            }}

            if (object.Equals(value1, null) || object.Equals(value2, null))
            {{
                return true;
            }}

            return value1.handle != value2.handle;
        }}
    }}
}}
";
            var className = $"{handleBaseName}Handle";
            var indentedFreeCode = string.Join(Environment.NewLine, freeCode.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Select((l)=>new string(' ',3*4)+l));
            var source = string.Format(sourceFormat, namespaceName, className, indentedFreeCode);
            return ($"{className}.g.cs", SourceText.From(source, Encoding.UTF8));
        }

        private bool MethodPredicate(SyntaxNode node, CancellationToken token)
        {
            if (node.IsKind(SyntaxKind.MethodDeclaration))
            {
                MethodDeclarationSyntax method = (MethodDeclarationSyntax)node;
                return method.Modifiers.Any(m => m.IsKind(SyntaxKind.ExternKeyword)) && method.AttributeLists.Count > 0;
            }
            return false;
        }

        private (string namespaceName, string className, string freeCode)? MethodTransformer(GeneratorSyntaxContext context, CancellationToken token)
        {
            var genAttrSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName($"{AttrNamespace}.{AttrName}");
            if (genAttrSymbol == null)
            {
                return null;
            }
            var freeMethodSymbol = (IMethodSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node, token)!;
            var genattr=freeMethodSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass != null && a.AttributeClass.Equals(genAttrSymbol, SymbolEqualityComparer.Default));
            if (genattr!=null)
            {
                return (freeMethodSymbol.ContainingNamespace.ToDisplayString(), (string)genattr.ConstructorArguments[0].Value!, GetFreeCode(freeMethodSymbol,context.SemanticModel.Compilation));
            }
            return null;
        }
        
        public string GetFreeCode()
        {
            return "return true;";
        }
        public string GetFreeCode(IMethodSymbol freeMethod, Compilation compilation)
        {
            var methodFormat = new SymbolDisplayFormat(memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);
            var returnFormat = new SymbolDisplayFormat(memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);
            var argFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
            var freeReturn = freeMethod.ReturnType;
            var argType = freeMethod.Parameters.First().Type;
            var freeArg = "this";
            if (argType.Equals(compilation.GetSpecialType(SpecialType.System_IntPtr), SymbolEqualityComparer.Default))
            {
                freeArg = "this.handle";
            }
            var methodcall = $"{freeMethod.ToDisplayString(methodFormat)}({freeArg})";
            if (freeReturn.Equals(compilation.GetSpecialType(SpecialType.System_Void), SymbolEqualityComparer.Default))
            {
                return $"{methodcall};\n{GetFreeCode()}";
            }
            else
            {
                string retval;
                if (freeReturn.Equals(compilation.GetSpecialType(SpecialType.System_Int32), SymbolEqualityComparer.Default) || freeReturn.TypeKind == TypeKind.Enum)
                {
                    retval = "0";
                    if (freeReturn.TypeKind == TypeKind.Enum)
                    {
                        var field = freeReturn.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(f => f.HasConstantValue && f.ConstantValue.Equals(0));
                        if (field != null)
                        {
                            retval = field.ToDisplayString(returnFormat);
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
                return $"return {methodcall} == {retval};";
            }
        } 
    }
}
