namespace iOSLib.SourceGenerator
{
    [Generator]
    class DefaultServiceSessionGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var source = context.SyntaxProvider.CreateSyntaxProvider(Predicate, Transformer).Where((c) => c != null);
            context.RegisterSourceOutput(source, Producer);
        }

        private bool Predicate(SyntaxNode node, CancellationToken token) => node.IsKind(SyntaxKind.ClassDeclaration) && ((ClassDeclarationSyntax)node).Identifier.Text.EndsWith("Base");
        private INamedTypeSymbol? Transformer(GeneratorSyntaxContext context, CancellationToken token)
        {
            var type = (ClassDeclarationSyntax)context.Node;
            var serviceSessionBase = context.SemanticModel.Compilation.GetTypeByMetadataName("IOSLib.ServiceSessionBase`2");
            var typeSymbol = context.SemanticModel.GetDeclaredSymbol(type);
            if (typeSymbol != null)
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
        private void Producer(SourceProductionContext context, INamedTypeSymbol? typeSymbol)
        {
            string nameSpaceName = typeSymbol!.ContainingNamespace.ToDisplayString();
            string className = typeSymbol.Name.Remove(typeSymbol.Name.Length - 4, 4);
            var sourceFormat = @"namespace {0}
{{
    public partial class {1} : {1}Base
    {{
        public {1}(IDevice device) : base(device) {{ }}
    }}
}}";
            context.AddSource($"{className}.g.cs", string.Format(sourceFormat, nameSpaceName, className));
        }

    }
}
