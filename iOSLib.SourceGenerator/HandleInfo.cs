﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace IOSLib.SourceGenerator
{
    internal class HandleInfo : SourceCodeInfoBase
    {
        internal HandleInfo(IMethodSymbol freeMethod, string handleBaseName)
        {
            FreeMethod = freeMethod;
            HandleBaseName = handleBaseName;
            namespaceName = FreeMethod.ContainingNamespace.ToDisplayString();
        }

        internal HandleInfo(string fullClassName)
        {
            var index = fullClassName.LastIndexOf(".");
            namespaceName = fullClassName.Substring(0,index);
            HandleBaseName = fullClassName.Substring(index+1);
        }

        internal IMethodSymbol FreeMethod { get; }

        private readonly string namespaceName;

        internal readonly string className;

        internal string HandleBaseName { get; }

        internal string HandleName => $"{HandleBaseName}Handle";

        private string GetFreeCode()
        {
            if (FreeMethod == null)
            {
                return "true";
            }
            else
            {
                var methodFormat = new SymbolDisplayFormat(memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);
                var returnFormat = new SymbolDisplayFormat(memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);
                var argFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
                var freeReturn = FreeMethod.ReturnType;
                string retval = "0";
                if (freeReturn.TypeKind == TypeKind.Enum)
                {
                    var field = freeReturn.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(f => f.HasConstantValue && f.ConstantValue.Equals(0));
                    if (field != null)
                    {
                        retval = field.ToDisplayString(returnFormat);
                    }
                }
                var argType = FreeMethod.Parameters.First().Type;
                var freeArg = "this";
                if (argType.ToDisplayString(argFormat) == "System.IntPtr")
                {
                    freeArg = "this.handle";
                }
                return (FreeMethod == null ? "true" : $"({FreeMethod.ToDisplayString(methodFormat)}({freeArg}) == {retval})");
            }
        }

        override internal IReadOnlyDictionary<string,string> BuildSource()
        {            
            var sourceFormat = @"using System;
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
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode=true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode=true)]
    public partial class {1} : IOSLib.Native.IOSHandle
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

        /// <inheritdoc/>
        [ReliabilityContractAttribute(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {{
            return {2};
        }}

        /// <inheritdoc/>
        public override string ToString()
        {{
            return $""{{this.handle}} (nameof({1}))"";
        }}

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {{
            if (obj != null && obj.GetType() == typeof({1}))
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
            var source = string.Format(sourceFormat, namespaceName, $"{HandleBaseName}Handle", GetFreeCode());
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string> { { $"{HandleName}.g.cs", source } });
        }
    }
}
