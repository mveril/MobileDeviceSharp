using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.SourceGenerator
{
    internal class UsedForReleaseProvidedResult
    {
        public ITypeSymbol ClassType { get; }
        public SyntaxList<StatementSyntax> FreeCode { get; }

        public UsedForReleaseProvidedResult(ITypeSymbol typeParameterSymbol, SyntaxList<StatementSyntax> statementSyntaxes)
        {
            ClassType = typeParameterSymbol;
            FreeCode = statementSyntaxes;
        }
    }
}
