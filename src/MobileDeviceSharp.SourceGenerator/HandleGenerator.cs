using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace MobileDeviceSharp.SourceGenerator
{
    [Generator]
    internal class HandleGenerator : IIncrementalGenerator
    {
        const string AttrNamespace = "MobileDeviceSharp.CompilerServices";
        const string AttrName = "UsedForReleaseAttribute`1";


        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var iosHandlesProvider = context.SyntaxProvider.CreateSyntaxProvider(IOSHandlesPredicate, IOSHandlesTransformer);
            var releaseMethodProvider = context.SyntaxProvider.CreateSyntaxProvider(ReleaseMethodPredicate, ReleaseMethodTransformer).Collect();
            var provider = iosHandlesProvider.Combine(releaseMethodProvider).Combine(context.CompilationProvider);
            context.RegisterSourceOutput(provider, Producer);
        }

        private void Producer(SourceProductionContext context, ((INamedTypeSymbol? handleType, ImmutableArray<UsedForReleaseProvidedResult?> methodData) syntaxSource, Compilation compilation) sourceData)
        {
            var ((handleType, usedForReleaseProviderResults), compilation) = sourceData;
            usedForReleaseProviderResults = usedForReleaseProviderResults.Where(x => x is not null).ToImmutableArray();
            if (handleType is null)
            {
                return;
            }
            MethodDeclarationSyntax freeMethod;
            var attributeDat = usedForReleaseProviderResults.FirstOrDefault(result => (result?.ClassType.Equals(handleType, SymbolEqualityComparer.Default)).GetValueOrDefault(false));
            if (attributeDat is null)
            {
                freeMethod = GetFreeMethod(compilation);
            }
            else
            {
                freeMethod = GetFreeMethod(attributeDat.FreeCode);
            }
            var (fileName, source) = GetSource(handleType, freeMethod);
            context.AddSource(fileName, source);
        }

        private bool IOSHandlesPredicate(SyntaxNode node, CancellationToken token)
        {
            if (node is ClassDeclarationSyntax classSyntax && classSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
            {
                return true;
            }
            return false;
        }

        private INamedTypeSymbol? IOSHandlesTransformer(GeneratorSyntaxContext context, CancellationToken token)
        {
            var type = (ClassDeclarationSyntax)context.Node;
            var IOSHandleBaseType = context.SemanticModel.Compilation.GetTypeByMetadataName("MobileDeviceSharp.Native.IOSHandle")!;
            var typeSymbol = context.SemanticModel.GetDeclaredSymbol(type)!;
            if (IOSHandleBaseType.Equals(typeSymbol.BaseType, SymbolEqualityComparer.Default))
            {
                return typeSymbol;
            }
            return null;
        }

        MethodDeclarationSyntax GetFreeMethod(Compilation compilation)
        {
            return GetFreeMethod(GetFreeCode(compilation));
        }

        MethodDeclarationSyntax GetFreeMethod(SyntaxList<StatementSyntax> statements)
        {
            return GetFreeMethodCore().WithBody(Block(statements));
        }

        MethodDeclarationSyntax GetFreeMethodCore()
        {
            var member = ParseMemberDeclaration(@"/// <inheritdoc/>
#if !NETCOREAPP
        [ReliabilityContractAttribute(Consistency.WillNotCorruptState, Cer.MayFail)]
#endif
        protected override bool ReleaseHandle()
        {

        }");
            System.Diagnostics.Debug.Assert(member is not null);
            return (MethodDeclarationSyntax)member!;
        }

        private (string name, SourceText source) GetSource(ITypeSymbol classType, MethodDeclarationSyntax ReleaseHandleDeclaration)
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
    public partial class {1} : MobileDeviceSharp.Native.IOSHandle
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

{2}

        /// <inheritdoc/>
        public override string ToString()
        {{
            return $""{{this.handle}} ({{nameof({1})}})"";
        }}

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {{
            if (obj is not null && obj.GetType() == typeof({1}))
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
            var className = classType.Name;
            var namespaceName = classType.ContainingNamespace;
            var source = string.Format(sourceFormat, namespaceName, className, ReleaseHandleDeclaration.NormalizeWhitespace().ToFullString());
            var srcstring = CSharpSyntaxTree.ParseText(source).GetRoot().NormalizeWhitespace().ToFullString();
            return ($"{className}.g.cs", SourceText.From(srcstring, Encoding.UTF8));
        }


        private bool ReleaseMethodPredicate(SyntaxNode node, CancellationToken token)
        {
            if (node.IsKind(SyntaxKind.MethodDeclaration))
            {
                MethodDeclarationSyntax method = (MethodDeclarationSyntax)node;
                return method.Modifiers.Any(m => m.IsKind(SyntaxKind.ExternKeyword) || m.IsKind(SyntaxKind.PartialKeyword) && method.AttributeLists.Count > 0);
            }
            return false;
        }

        private UsedForReleaseProvidedResult? ReleaseMethodTransformer(GeneratorSyntaxContext context, CancellationToken token)
        {
            var setFreeAttrSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName($"{AttrNamespace}.{AttrName}");
            if (setFreeAttrSymbol == null)
            {
                return null;
            }
            var currentSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node, token)!;
            if (currentSymbol is not IMethodSymbol freeMethodSymbol)
            {
                return null;
            }
            var attributes = freeMethodSymbol.GetAttributes();
            var setFreeAttr = attributes.FirstOrDefault((a) => a.AttributeClass is not null && a.AttributeClass.IsGenericType && a.AttributeClass.OriginalDefinition.Equals(setFreeAttrSymbol, SymbolEqualityComparer.Default));
            if (setFreeAttr is not null)
            {
                return new UsedForReleaseProvidedResult(setFreeAttr.AttributeClass!.TypeArguments[0].OriginalDefinition, GetFreeCode(freeMethodSymbol, context.SemanticModel.Compilation));
            }
            return null;
        }

        public static readonly ReturnStatementSyntax s_defaultReturnStatement = ReturnStatement(LiteralExpression(SyntaxKind.TrueLiteralExpression));

        public SyntaxList<StatementSyntax> GetFreeCode(Compilation compilation)
        {
            var nativeMemoryType = compilation.GetTypeByMetadataName("System.Runtime.InteropServices.NativeMemory");
            IMethodSymbol? freeMethod = null;
            if (nativeMemoryType is not null)
            {
                freeMethod = nativeMemoryType.GetMembers().OfType<IMethodSymbol>().FirstOrDefault((m) => m.IsStatic && m.Name == "Free");
            }
            if (freeMethod is null)
            {
                var marshallType = compilation.GetTypeByMetadataName("System.Runtime.InteropServices.Marshal")!;
                freeMethod = marshallType.GetMembers().OfType<IMethodSymbol>().FirstOrDefault((m) => m.IsStatic && m.Name == "FreeHGlobal");
            }
            return GetFreeCode(freeMethod, compilation);
        }

        public SyntaxList<StatementSyntax> GetFreeCode(IMethodSymbol freeMethod, Compilation compilation)
        {
            var methodFormat = new SymbolDisplayFormat(memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);
            var returnFormat = new SymbolDisplayFormat(memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);
            var argFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
            INamedTypeSymbol voidType = compilation.GetSpecialType(SpecialType.System_Void);
            INamedTypeSymbol intPtrType = compilation.GetSpecialType(SpecialType.System_IntPtr);
            IPointerTypeSymbol voidPointerType = compilation.CreatePointerTypeSymbol(voidType);
            var freeReturn = freeMethod.ReturnType;
            var argType = freeMethod.Parameters.First().Type;
            var needUnsafe = false;
            var statements =new List<StatementSyntax>();
            var freeArg = Argument(ThisExpression());
            var handleAccess = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
    ThisExpression(), IdentifierName("handle"));
            if (argType.Equals(intPtrType, SymbolEqualityComparer.Default))
            {
                freeArg = Argument(handleAccess);
            }
            else if(argType.Equals(voidPointerType, SymbolEqualityComparer.Default))
            {
                var toPointerAcess = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    handleAccess, IdentifierName("ToPointer"));
                var invocation = InvocationExpression(toPointerAcess);
                freeArg = Argument(invocation);
                needUnsafe = true;
            }
            var methodcall = InvocationExpression(ParseExpression(freeMethod.ToDisplayString(methodFormat))).AddArgumentListArguments(freeArg);
            StatementSyntax callStatement;
            if (freeReturn.Equals(compilation.GetSpecialType(SpecialType.System_Void), SymbolEqualityComparer.Default))
            {
                callStatement = ExpressionStatement(methodcall);
            }
            else
            {
                ExpressionSyntax retval;
                if (freeReturn.Equals(compilation.GetSpecialType(SpecialType.System_Int32), SymbolEqualityComparer.Default) || freeReturn.TypeKind == TypeKind.Enum)
                {
                    retval = LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0));
                    if (freeReturn.TypeKind == TypeKind.Enum)
                    {
                        var field = freeReturn.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(f => f.HasConstantValue && f.ConstantValue.Equals(0));
                        if (field is not null)
                        {
                            retval = ParseExpression(field.ToDisplayString(returnFormat));
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
                callStatement = ReturnStatement(BinaryExpression(SyntaxKind.EqualsExpression,
                    methodcall,
                    retval));
            }
            statements.Add(needUnsafe ? UnsafeStatement().WithBlock(Block(SingletonList(callStatement))) : callStatement);
            if (!callStatement.IsKind(SyntaxKind.ReturnStatement))
            {
                statements.Add(s_defaultReturnStatement);
            }
            return List(statements);
        }
    }
}
