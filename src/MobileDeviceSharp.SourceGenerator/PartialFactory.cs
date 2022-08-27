using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MobileDeviceSharp.SourceGenerator
{
    internal static class PartialFactory
    {
        private static IEnumerable<MemberDeclarationSyntax> ParentNodes(MemberDeclarationSyntax memberDeclaration)
        {
            for (SyntaxNode? node = memberDeclaration; node is not null && typeof(MemberDeclarationSyntax).IsAssignableFrom(node.GetType()); node = node.Parent)
            {
                yield return (MemberDeclarationSyntax)node;
            }
        }

        public static MemberDeclarationSyntax CreatePartial(MethodDeclarationSyntax methodDeclaration, Func<MethodDeclarationSyntax, MethodDeclarationSyntax> builder)
        {
            if (methodDeclaration.Parent is TypeDeclarationSyntax type)
            {

            }
            else
            {
                throw new NotSupportedException();
            }
            var childs = List<MemberDeclarationSyntax>(new[] { builder(methodDeclaration) });
            return CreatePartial(type, (type) => type.WithMembers(childs));
        }

        public static MemberDeclarationSyntax CreatePartial<T>(T typeDeclaration, Func<T,T> builder) where T : TypeDeclarationSyntax
        {
            var nodes = ParentNodes(typeDeclaration);
            MemberDeclarationSyntax? child = null;
            SyntaxList<MemberDeclarationSyntax> childs;
            foreach (var node in nodes)
            {
                if (child==null)
                {
                    childs = List<MemberDeclarationSyntax>();
                }
                else
                {
                    if (child is TypeDeclarationSyntax typeDeclarationSyntax && typeDeclarationSyntax.Identifier.ToString()== typeDeclaration.Identifier.ToString())
                    {
                        child = builder(typeDeclaration);
                    }
                    childs = List(new MemberDeclarationSyntax[] {child});
                }
                if(node is TypeDeclarationSyntax typeNode)
                {
                    child = typeNode.Clean().WithMembers(childs);
                }
                else if (node is NamespaceDeclarationSyntax namespaceNode)
                {
                    child = namespaceNode.WithMembers(childs);
                }
            }
            return child!;

        }
        private static TypeDeclarationSyntax Clean(this TypeDeclarationSyntax type)
        {
            var result = type.WithAttributeLists(List<AttributeListSyntax>())
                .WithBaseList(null);
            if (!result.Modifiers.Any(m=>m.IsKind(SyntaxKind.PartialKeyword)))
            {
                result = result.AddModifiers(Token(SyntaxKind.PartialKeyword));
            }
            return result;
        }
    }
}
