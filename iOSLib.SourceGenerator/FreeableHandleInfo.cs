using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace IOSLib.SourceGenerator
{
    internal class FreeableHandleInfo : HandleInfoBase
    {
        internal IMethodSymbol FreeMethod { get; }

        private readonly Compilation _compilation;

        internal FreeableHandleInfo(IMethodSymbol freeMethod, string handleBaseName, Compilation compilation) : base(freeMethod.ContainingNamespace.ToDisplayString(), handleBaseName)
        {
            _compilation = compilation;
            FreeMethod = freeMethod;
        }

        protected override string GetFreeCode()
        {
            var methodFormat = new SymbolDisplayFormat(memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);
            var returnFormat = new SymbolDisplayFormat(memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);
            var argFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
            var freeReturn = FreeMethod.ReturnType;
            var argType = FreeMethod.Parameters.First().Type;
            var freeArg = "this";
            if (argType.Equals(_compilation.GetSpecialType(SpecialType.System_IntPtr), SymbolEqualityComparer.Default))
            {
                freeArg = "this.handle";
            }
            var methodcall = $"{FreeMethod.ToDisplayString(methodFormat)}({freeArg})";
            if (freeReturn.Equals(_compilation.GetSpecialType(SpecialType.System_Void), SymbolEqualityComparer.Default))
            {
                return $"{methodcall};\nreturn {DefaultReturn};";
            }
            else
            {
                string retval;
                if (freeReturn.Equals(_compilation.GetSpecialType(SpecialType.System_Int32), SymbolEqualityComparer.Default) || freeReturn.TypeKind == TypeKind.Enum)
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
                return $"return ({methodcall} == {retval});";
            }
        }
    }
}
