namespace IOSLib.SourceGenerator
{
    [Generator]
    class DefaultServiceSessionGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var source = context.SyntaxProvider.CreateSyntaxProvider(Predicate, Transformer).Where((c) => c is not null);
            context.RegisterSourceOutput(source.Collect(), Producer);
        }

        private bool Predicate(SyntaxNode node, CancellationToken token) => node.IsKind(SyntaxKind.ClassDeclaration) && ((ClassDeclarationSyntax)node).Identifier.Text.EndsWith("Base");
        private INamedTypeSymbol? Transformer(GeneratorSyntaxContext context, CancellationToken token)
        {
            var type = (ClassDeclarationSyntax)context.Node;
            var serviceSessionBase = context.SemanticModel.Compilation.GetTypeByMetadataName("IOSLib.ServiceSessionBase`2");
            var typeSymbol = context.SemanticModel.GetDeclaredSymbol(type);
            if (typeSymbol is not null)
            {
                var bt = typeSymbol.BaseType;
                var btbt = bt?.OriginalDefinition;
                if ((btbt?.IsGenericType).GetValueOrDefault(false))
                {
                    if (btbt!.OriginalDefinition.Equals(serviceSessionBase, SymbolEqualityComparer.Default))
                    {
                        return typeSymbol;
                    }
                }
            }
            return null;
        }

        private void Producer(SourceProductionContext context, ImmutableArray<INamedTypeSymbol?> typeSymbols)
        {
            
            foreach (INamedTypeSymbol item in typeSymbols.Distinct(SymbolEqualityComparer.Default))
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    break;
                }
                Producer(context, item);
            }
        }

        private void Producer(SourceProductionContext context, INamedTypeSymbol? typeSymbol)
        {
            string nameSpaceName = typeSymbol!.ContainingNamespace.ToDisplayString();
            string className = typeSymbol.Name.Remove(typeSymbol.Name.Length - 4, 4);
            var param = SyntaxFactory.Identifier("device");
            var source = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(nameSpaceName))
                .AddMembers(
                    SyntaxFactory.ClassDeclaration(className)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                    .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(typeSymbol.Name)))
                    .AddMembers(
                        SyntaxFactory.ConstructorDeclaration(className)
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                        .WithParameterList(
                            SyntaxFactory.ParameterList()
                            .AddParameters(
                                SyntaxFactory.Parameter(param)
                                .WithType(
                                    SyntaxFactory.ParseTypeName("IDevice"))))
                        .WithInitializer(
                            SyntaxFactory.ConstructorInitializer(
                                SyntaxKind.BaseConstructorInitializer, SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(SyntaxFactory.Argument(
                                        SyntaxFactory.IdentifierName(param))))))
                        .WithBody(SyntaxFactory.Block())));
            var src = source.NormalizeWhitespace().ToFullString();
            context.AddSource($"{className}.g.cs", src);
        }

    }
}
