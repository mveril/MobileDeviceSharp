namespace MobileDeviceSharp.SourceGenerator
{
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    [Generator]
    class NotificationProxyEventMappingGenerator : IIncrementalGenerator
    {

        internal const string AttrNamespace = "MobileDeviceSharp.CompilerServices";
        internal const string AttrName = "NotificationProxyEventNameAttribute";
        internal const string AttributeFullName = $"{AttrNamespace}.{AttrName}";
        internal const string PropName = "Name";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            //Debugger.Launch();
            var eventProvider = context.SyntaxProvider.CreateSyntaxProvider(Predicate, Transformer).Where(eventData=>eventData!=null);
            var provider = eventProvider.Collect();
            context.RegisterSourceOutput(provider, Execute);
        }

        private static TypeDeclarationSyntax Factory(TypeDeclarationSyntax classDeclaration,IEnumerable<NpEventSyntaxPartsBuilder> npEventBuilders)
        {
            var memberlist = new List<MemberDeclarationSyntax>();
            var npField = FieldDeclaration(VariableDeclaration(ParseTypeName("NotificationProxySession"), SingletonSeparatedList(VariableDeclarator("_np")))).AddModifiers(Token(SyntaxKind.PrivateKeyword));
            var handlerIdentifer = Identifier("NotificationProxyHandler");
            var deviceIdentifier = Identifier("device");
            memberlist.Add(npField);
            memberlist.Add(MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), "InitEventWatching")
                .AddModifiers(Token(SyntaxKind.PrivateKeyword))
                .AddParameterListParameters(Parameter(deviceIdentifier).WithType(ParseTypeName("IDevice")))
                .AddBodyStatements(
                    ExpressionStatement(
                        AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            IdentifierName(npField.Declaration.Variables[0].Identifier),
                            ObjectCreationExpression(npField.Declaration.Type).AddArgumentListArguments(Argument(IdentifierName(deviceIdentifier))))),
                   ExpressionStatement(
                       AssignmentExpression(
                           SyntaxKind.AddAssignmentExpression,
                           MemberAccessExpression(
                               SyntaxKind.SimpleMemberAccessExpression,
                               IdentifierName(npField.Declaration.Variables[0].Identifier),
                               IdentifierName("NotificationProxyEvent")),
                           IdentifierName(handlerIdentifer))),
                   ExpressionStatement(
                       InvocationExpression(
                           MemberAccessExpression(
                               SyntaxKind.SimpleMemberAccessExpression,
                               IdentifierName(npField.Declaration.Variables[0].Identifier),
                               IdentifierName("ObserveNotification")), ArgumentList(SeparatedList(npEventBuilders.Select(builder => builder.ObserveArgument)))))));
            memberlist.Add(MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), handlerIdentifer)
                .AddModifiers(Token(SyntaxKind.PrivateKeyword))
                .AddParameterListParameters(
                Parameter(
                    Identifier("sender")).
                    WithType(
                    PredefinedType(Token(SyntaxKind.ObjectKeyword))),
                Parameter(
                    Identifier("e")).
                    WithType(ParseTypeName("NotificationProxyEventArgs")))
                .AddBodyStatements(
                    SwitchStatement(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("e"),
                            IdentifierName("EventName")))
                    .WithSections(List(npEventBuilders.Select(builder => builder.SwitchSection))
                    .Add(SwitchSection().AddLabels(DefaultSwitchLabel()).AddStatements(BreakStatement())))));
            memberlist.AddRange(npEventBuilders.Select(builder => builder.OnMethod));
            memberlist.Add(MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), "CloseEventWatching")
                .AddModifiers(Token(SyntaxKind.PrivateKeyword))
                .AddBodyStatements(
                    ExpressionStatement(
                        InvocationExpression(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(npField.Declaration.Variables[0].Identifier),
                            IdentifierName("Dispose"))))));
            return classDeclaration.WithMembers(List(memberlist)).WithBaseList(null);
        }

        private void Execute(SourceProductionContext context, ImmutableArray<(INamedTypeSymbol classInfo,NpEventSyntaxPartsBuilder eventSyntaxPartsBuilder)?> events)
        {
            var classEvents=events.GroupBy(eventData=>eventData.Value!.classInfo,(eventData)=>eventData.Value.eventSyntaxPartsBuilder);
            foreach (var group in classEvents)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;
                var classInfo = group.Key;
                var classSyntax = classInfo.DeclaringSyntaxReferences.First()?.GetSyntax(context.CancellationToken) as TypeDeclarationSyntax;
                if (classSyntax==null)
                {
                    continue;
                }
                var npEventBuilders = group;

                var myclass = PartialFactory.CreatePartial(classSyntax,(syntax)=> Factory(syntax,npEventBuilders));
                var source = CompilationUnit().AddUsings(UsingDirective(IdentifierName("System")),UsingDirective(IdentifierName("MobileDeviceSharp.NotificationProxy"))).AddMembers(myclass);
                context.AddSource($"{classInfo.Name}.np.g.cs",SourceText.From(source.NormalizeWhitespace().ToFullString(),Encoding.UTF8));
            }
        }

        private static bool Predicate(SyntaxNode node, CancellationToken token)
        {
            return node.IsKind(SyntaxKind.EventFieldDeclaration) && ((EventFieldDeclarationSyntax)node).AttributeLists.Count > 0 && node.Parent.IsKind(SyntaxKind.ClassDeclaration);
        }

        private static (INamedTypeSymbol classinfo, NpEventSyntaxPartsBuilder eventSyntaxPartsBuilder)? Transformer(GeneratorSyntaxContext context, CancellationToken token)
        {
            var declaration = (EventFieldDeclarationSyntax)context.Node;
            var node = declaration.Declaration.Variables.First();
            var symbol = context.SemanticModel.GetDeclaredSymbol(node);
            IEventSymbol eventSymbol = (IEventSymbol)symbol!;
            var eventName = eventSymbol.Name;
            foreach (var attribute in eventSymbol.GetAttributes())
            {
                if (token.IsCancellationRequested)
                    break;
                string fullName = attribute.AttributeClass!.ToDisplayString();
                // Is the attribute the [NotificationProxyEventNameAttribute] attribute?
                if (fullName == AttributeFullName)
                {
                    // return the event
                    var eventID = attribute.ConstructorArguments[0].Value!.ToString();
                    return ((INamedTypeSymbol)eventSymbol.ContainingSymbol!, new NpEventSyntaxPartsBuilder(eventID, eventName));
                }
            }
            return null;
        }
    }
}
