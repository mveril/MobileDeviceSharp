namespace iOSLib.SourceGenerator
{

    [Generator]
    class NotificationProxyEventMappingGenerator : IIncrementalGenerator
    {

        internal const string AttrNamespace = "IOSLib.CompilerServices";
        internal const string AttrName = "NotificationProxyEventNameAttribute";
        internal const string AttributeFullName = $"{AttrNamespace}.{AttrName}";
        internal const string PropName = "Name";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            //Debugger.Launch();
            var eventProvider = context.SyntaxProvider.CreateSyntaxProvider(Predicate, Transformer).Where(e=>e!=null);
            var provider = context.CompilationProvider.Combine(eventProvider.Collect());
            context.RegisterSourceOutput(provider, (context, source) => Execute(context, source.Left, source.Right));
        }

        private const string CodeStruct= @"using System;

namespace {0}
{{
    partial class {1}
    {{
        private NotificationProxySession _np;
        private void InitEventWatching(IDevice device)
        {{
            _np = new NotificationProxySession(device);
            _np.NotificationProxyEvent += NotificationProxyHandler;
            _np.ObserveNotification({2});
        }}
        
        private void NotificationProxyHandler(object sender, NotificationProxyEventArgs e)
        {{
            switch(e.EventName)
            {{
{3}
            }};
        }}

{4}

        private void CloseEventWatching()
        {{
            _np.Dispose();
        }}
    }}
}}";

        private void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<IEventSymbol> events)
        {
            INamedTypeSymbol npNameAttrSymbol = compilation.GetTypeByMetadataName(AttributeFullName)!;
            string GetSwitchCode(IEnumerable<IEventSymbol> events)
            {
                var sb = new StringBuilder();
                foreach (var item in events)
                {
                    var npName = item.GetAttributes().First(a => a.AttributeClass!.Equals(npNameAttrSymbol, SymbolEqualityComparer.Default)).ConstructorArguments.First().Value;
                    sb.AppendLine($"                case \"{npName}\":");
                    sb.AppendLine($"                    On{item.Name}();");
                    sb.AppendLine($"                    break;");
                }
                sb.AppendLine($"                default:");
                sb.AppendLine($"                    break;");
                return sb.ToString();
            }

            string GetFunctionsCode(IEnumerable<IEventSymbol> events)
            {
                var funcs = events.Select(ev =>
                {
                    return $@"        private void On{ev.Name}()
        {{
            {ev.Name}?.Invoke(this, EventArgs.Empty);
        }}";
                });
                return string.Join("\n        \n", funcs);
            }
            string ObserveCode(IEnumerable<IEventSymbol> events)
            {
                var lists = events.Select(ev => $"\"{ev.GetAttributes().First(a => a.AttributeClass.Equals(npNameAttrSymbol, SymbolEqualityComparer.Default)).ConstructorArguments.First().Value}\"");
                return string.Join(", ", lists);
            }
            var classEvents=events.GroupBy(e=>e.ContainingType);
            foreach (var group in classEvents)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;
                var classInfo = group.Key;
                context.AddSource($"{classInfo.Name}.np.g.cs", string.Format(CodeStruct, classInfo.ContainingNamespace, classInfo.Name, ObserveCode(group), GetSwitchCode(group), GetFunctionsCode(group)));
            }
        }

        private static bool Predicate(SyntaxNode node, CancellationToken token)
        {
            return node.IsKind(SyntaxKind.EventFieldDeclaration) && ((EventFieldDeclarationSyntax)node).AttributeLists.Count > 0 && node.Parent.IsKind(SyntaxKind.ClassDeclaration);
        }

        private static IEventSymbol? Transformer(GeneratorSyntaxContext context, CancellationToken token)
        {
            var declaration = (EventFieldDeclarationSyntax)context.Node;
            var node = declaration.Declaration.Variables.First();
            var symbol = context.SemanticModel.GetDeclaredSymbol(node);
            IEventSymbol eventSymbol = (IEventSymbol)symbol!;
            foreach (var attribute in eventSymbol.GetAttributes())
            {
                if (token.IsCancellationRequested)
                    break;
                string fullName = attribute.AttributeClass!.ToDisplayString();
                // Is the attribute the [NotificationProxyEventNameAttribute] attribute?
                if (fullName == AttributeFullName)
                {
                    // return the event
                    return eventSymbol;
                    }
                }
                return null;
        }
    }
}
