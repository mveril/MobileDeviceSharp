using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IOSLib.SourceGenerator
{
    internal class NpEventSyntaxPartsBuilder
    {
        private readonly Lazy<MethodDeclarationSyntax> _onMethod;

        public MethodDeclarationSyntax OnMethod => _onMethod.Value;

        private readonly Lazy<SwitchSectionSyntax> _switchSection;

        public SwitchSectionSyntax SwitchSection => _switchSection.Value;

        private readonly Lazy<ArgumentSyntax> _observeArgument;

        public ArgumentSyntax ObserveArgument => _observeArgument.Value;

        private readonly Lazy<SyntaxToken> _onMethodIdentifier;
        public string EventName { get; }
        public string EventID { get; }

        public NpEventSyntaxPartsBuilder(string eventID, string eventName)
        {

            EventID = eventID;
            EventName = eventName;
            _onMethod = new Lazy<MethodDeclarationSyntax>(OnMethodFactory);
            _switchSection = new Lazy<SwitchSectionSyntax>(SwitchSectionFactory);
            _observeArgument = new Lazy<ArgumentSyntax>(ObserveArgumentFactory);
            _onMethodIdentifier = new Lazy<SyntaxToken>(()=>Identifier($"On{EventName}"));
        }

        private MethodDeclarationSyntax OnMethodFactory()
        {
            return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), _onMethodIdentifier.Value)
                .AddModifiers(Token(SyntaxKind.PrivateKeyword))
                .AddBodyStatements(ExpressionStatement(ParseExpression($"{EventName}?.Invoke(this, EventArgs.Empty)")));
        }
        private SwitchSectionSyntax SwitchSectionFactory()
        {
            return SwitchSection()
                .AddLabels(
                    CaseSwitchLabel(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(EventID))))
                .AddStatements(
                    ExpressionStatement(InvocationExpression(IdentifierName(_onMethodIdentifier.Value))),
                    BreakStatement());
        }
        private ArgumentSyntax ObserveArgumentFactory()
        {
            return Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(EventID)));
        }
    }
}
