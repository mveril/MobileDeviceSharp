﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDevice.NET.SourceGenerator
{
    internal class HandleInfo
    {
        internal HandleInfo(MethodDeclarationSyntax freeMethod, string handleBaseName)
        {
            FreeMethod = freeMethod;
            HandleBaseName = handleBaseName;
        }
        internal MethodDeclarationSyntax FreeMethod { get; }
        internal string HandleBaseName { get; }
        internal string HandleName => $"{HandleBaseName}Handle";
        internal string BuildSource()
        {
            NamespaceDeclarationSyntax namespaceDeclarationSyntax = null;
            if (!SyntaxNodeHelper.TryGetParentSyntax(this.FreeMethod, out ClassDeclarationSyntax classDeclaration))
            {
                return null;
            }
            if (!SyntaxNodeHelper.TryGetParentSyntax(classDeclaration, out NamespaceDeclarationSyntax namespaceDeclaration))
            {
                return null;
            }
            
            var namespaceName = namespaceDeclarationSyntax.Name.ToString();
            var className = classDeclaration.Identifier.ToString();
            var methodName = this.FreeMethod.Identifier.ToString();
            var fullFreeMethodName = $"{namespaceName}.{className}.{fullMethodName";
            var source = @"using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace {0}
{
    /// <summary>
    /// Represents a wrapper class for iDevice handles.
    /// </summary>
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode=true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode=true)]
    public partial class {1} : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref=""{1}""/> class.
        /// </summary>
        protected iDeviceHandle() :
                base(true)
        {{

        }}

            /// <summary>
            /// Initializes a new instance of the <see cref=""{1}""/> class, specifying whether the handle is to be reliably released.
            /// </summary>
            /// <param name=""ownsHandle"">
            /// <see langword=""true""/> to reliably release the handle during the finalization phase; <see langword=""false""/> to prevent reliable release (not recommended).
            /// </param>
            protected {1}(bool ownsHandle) :
                base(ownsHandle)
            {{
                
            }}
        /// <summary>
        /// Gets a value which represents a pointer or handle that has been initialized to zero.
        /// </summary>
        public static iDeviceHandle Zero
        {{
            get
            {{
                return {1}.DangerousCreate(System.IntPtr.Zero);
            }}
        }}

        /// <inheritdoc/>
        [ReliabilityContractAttribute(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {{
            return ({2}(this.handle) == iDeviceError.Success);
        }}

        /// <summary>
        /// Creates a new <see cref=""{1}""/> from a <see cref=""IntPtr""/>.
        /// </summary>
        /// <param name=""unsafeHandle"">
        /// The underlying <see cref=""IntPtr""/>
        /// </param>
        /// <param name=""ownsHandle"">
        /// <see langword=""true""/> to reliably release the handle during the finalization phase; <see langword=""false""/> to prevent reliable release (not recommended).
        /// </param>
        /// <returns>
        /// </returns>
        public static {1} DangerousCreate(System.IntPtr unsafeHandle, bool ownsHandle)
        {{
            {1} safeHandle = new {1}(ownsHandle);
            safeHandle.SetHandle(unsafeHandle);
            return safeHandle;
        }}

        /// <summary>
        /// Creates a new <see cref=""iDeviceHandle""/> from a <see cref=""IntPtr""/>.
        /// </summary>
        /// <param name=""unsafeHandle"">
        /// The underlying <see cref=""IntPtr""/>
        /// </param>
        /// <returns>
        /// </returns>
        public static {0} DangerousCreate(System.IntPtr unsafeHandle)
        {{
            return {1}.DangerousCreate(unsafeHandle, true);
        }}

        /// <inheritdoc/>
        public override string ToString()
        {{
            return $""{this.handle} ({1})"";
        }}

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {{
            if (obj != null && obj.GetType() == typeof({0}))
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
        public static bool operator !=(iDeviceHandle value1, iDeviceHandle value2)
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
            return string.Format(source, namespaceName, HandleBaseName, fullFreeMethodName);
        }
    }
}
