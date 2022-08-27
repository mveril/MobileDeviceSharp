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
    public partial class LockdownSession
    {
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

            public LockdownSession Session { get; }
            public string? Name { get; }

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
            /// Try get a value for and <paramref name="key"/>.
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

            public PlistDictionary ToDictionary()
            {
                lockdownd_get_value(Session.Handle, Name, null, out var plistHandle);
                return new PlistDictionary(plistHandle);
            }

            public static explicit operator PlistDictionary(LockdownDomain lockdownDomain) => lockdownDomain.ToDictionary();
        }
    }
}
