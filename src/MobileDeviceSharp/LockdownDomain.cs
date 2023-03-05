using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using MobileDeviceSharp.Native;
using MobileDeviceSharp.PropertyList;
using static MobileDeviceSharp.Native.Lockdown;

namespace MobileDeviceSharp
{
    public sealed partial class LockdownSession
    {
        /// <summary>
        /// Represent a lockdown domain used to get specific type of value from the <see cref="LockdownSession"/>.
        /// </summary>
        public class LockdownDomain
        {
            internal LockdownDomain(LockdownSession session, string? domainName)
            {
                Session = session;
                Name = domainName;
                using var dic = ToDictionary();
                if (dic.Count == 0)
                {
                    throw new ArgumentException($"The domain {domainName} not exist",nameof(domainName));

                }
            }

            internal LockdownDomain(LockdownSession session) : this(session, null)
            {

            }

            /// <summary>
            /// The <see cref="LockdownSession"/> used to create this Lockdown domain.
            /// </summary>
            public LockdownSession Session { get; }

            /// <summary>
            /// The name of the LockdownDomain.
            /// </summary>
            public string? Name { get; }

            /// <summary>
            /// Get a value from the device associated to this <see cref="LockdownDomain"/>.
            /// </summary>
            /// <param name="key"></param>
            /// <returns>The requested value.</returns>
            public PlistNode this[string key]
            {
                get
                {

                    var hresult = lockdownd_get_value(Session.Handle, Name, key, out var plistHandle);
                    if (hresult.IsError())
                    {
                        throw hresult.GetException();
                    }
                    return PlistNode.From(plistHandle)!;
                }
                set
                {
                    var hresult = lockdownd_set_value(Session.Handle, Name, key, value.Handle);
                    if (hresult.IsError())
                    {
                        throw hresult.GetException();
                    }
                }
            }

            /// <summary>
            /// Try get a value for the <paramref name="key"/>.
            /// </summary>
            /// <param name="key">The target key.</param>
            /// <param name="node">The result <see cref="PlistNode"/></param>
            /// <returns>True on sucess</returns>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            public bool TryGetValue(string key, [MaybeNullWhen(false)] out PlistNode node)
#else
            public bool TryGetValue(string key, out PlistNode node)
#endif
            {
                lockdownd_get_value(Session.Handle, Name, key, out var plistHandle);
                node = PlistNode.From(plistHandle);
                return node is not null;
            }

            /// <summary>
            /// Try set value for and <paramref name="key"/>.
            /// </summary>
            /// <param name="key">The target key.</param>
            /// <param name="node">The result <see cref="PlistNode"/></param>
            /// <returns>The lockdownError</returns>
            public LockdownError TrySetValue(string key, PlistNode node)
            {
                var err = lockdownd_set_value(Session.Handle, Name, key, node.Handle);
                return err;
            }

            /// <summary>
            /// Get a <see cref="PlistDictionary"/> representation of this <see cref="LockdownDomain"/>.
            /// </summary>
            /// <returns></returns>
            public PlistDictionary ToDictionary()
            {
                lockdownd_get_value(Session.Handle, Name, null, out var plistHandle);
                return new PlistDictionary(plistHandle);
            }

            /// <summary>
            /// Convert the <see cref="LockdownDomain"/> to a <see cref="PlistDictionary"/> representation.
            /// </summary>
            /// <param name="lockdownDomain">The targeted domain.</param>
            public static explicit operator PlistDictionary(LockdownDomain lockdownDomain) => lockdownDomain.ToDictionary();
        }
    }
}
