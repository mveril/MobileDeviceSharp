﻿using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace MobileDeviceSharp.SourceGenerator
{
    [Generator]
    internal class HandleGenerator : IIncrementalGenerator
    {
        const string AttrNamespace = "MobileDeviceSharp.CompilerServices";
        const string AttrName = "GenerateHandleAttribute";


        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var nonFreeableFullNames = context.AdditionalTextsProvider
                .Where(file => Path.GetFileName(file.Path).Equals("GenerateHandle.txt", StringComparison.OrdinalIgnoreCase))
                .Select((file, token) => file.GetText())
                .Where((text) => text is not null)
                .SelectMany((text, token) => text!.Lines)
                .Where((line) => !line.Span.IsEmpty)
                .Select((line, token) => line.Text!.ToString(line.Span));
            var nonfreeableProvider = nonFreeableFullNames.Combine(context.CompilationProvider);
            context.RegisterSourceOutput(nonfreeableProvider, NonFreeableProducer);
            var freeableMethods = context.SyntaxProvider.CreateSyntaxProvider(MethodPredicate, MethodTransformer);
            context.RegisterSourceOutput(freeableMethods, FreeableProducer);
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

        private void FreeableProducer(SourceProductionContext context, (string namespaceName, string className, SyntaxList<StatementSyntax> freeCode)? methodData)
        {

            if (methodData.HasValue)
            {
                var (nameSpace, className, freeCode) = methodData.Value;
                var (fileName, source) = GetSource(nameSpace, className, GetFreeMethod(freeCode));
                context.AddSource(fileName, source);
            }
        }

        private void NonFreeableProducer(SourceProductionContext context, (string fullClassName, Compilation compilation) data)
        {
            var index = data.fullClassName.LastIndexOf(".");
            var namespaceName = data.fullClassName.Substring(0, index);
            var handleBaseName = data.fullClassName.Substring(index + 1);
            var (fileName, source) = GetSource(namespaceName, handleBaseName, GetFreeMethod(data.compilation));
            context.AddSource(fileName, source);
        }

        private (string name, SourceText source) GetSource(string namespaceName, string handleBaseName, MethodDeclarationSyntax ReleaseHandleDeclaration)
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
    public sealed partial class {1} : MobileDeviceSharp.Native.IOSHandle
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
            var className = $"{handleBaseName}Handle";
            var source = string.Format(sourceFormat, namespaceName, className, ReleaseHandleDeclaration.NormalizeWhitespace().ToFullString());
            var srcstring = CSharpSyntaxTree.ParseText(source).GetRoot().NormalizeWhitespace().ToFullString();
            return ($"{className}.g.cs", SourceText.From(srcstring, Encoding.UTF8));
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

        private (string namespaceName, string className, SyntaxList<StatementSyntax> freeCode)? MethodTransformer(GeneratorSyntaxContext context, CancellationToken token)
        {
            var genAttrSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName($"{AttrNamespace}.{AttrName}");
            if (genAttrSymbol == null)
            {
                return null;
            }
            var freeMethodSymbol = (IMethodSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node, token)!;
            var genattr=freeMethodSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass is not null && a.AttributeClass.Equals(genAttrSymbol, SymbolEqualityComparer.Default));
            if (genattr!=null)
            {
                return (freeMethodSymbol.ContainingNamespace.ToDisplayString(), (string)genattr.ConstructorArguments[0].Value!, GetFreeCode(freeMethodSymbol, context.SemanticModel.Compilation));
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
