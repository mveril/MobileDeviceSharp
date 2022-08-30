using MobileDeviceSharp.PropertyList.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList
{
    public partial class PlistArray
    {
        /// <inheritdoc/>
        private sealed class Enumerator : IEnumerator<PlistNode>
        {
            private readonly PlistArray _root;
            private PlistArrayIterHandle _iterHandle;

            /// <inheritdoc/>
            public Enumerator(PlistArray root)
            {
                _root = root;
                plist_array_new_iter(_root.Handle, out _iterHandle);
            }

#pragma warning disable IDE0032 // Use auto-property (auto readonly property not working because we set the _current in MoveNext)
            private PlistNode _current;
#pragma warning restore IDE0032 // Use auto-property (auto readonly property not working because we set the _current in MoveNext)

            /// <inheritdoc/>
            public PlistNode Current => _current;

            object IEnumerator.Current => _current;

            /// <inheritdoc/>
            public void Dispose()
            {
                _iterHandle.Dispose();
            }

            /// <inheritdoc/>
            public bool MoveNext()
            {
                plist_array_next_item(_root.Handle, _iterHandle, out var currentHandle);
                var success = !currentHandle.IsInvalid;
                if (success)
                {
                    _current = From(currentHandle)!;
                }
                return success;
            }

            /// <inheritdoc/>
            public void Reset()
            {
                _iterHandle.Dispose();
                plist_array_new_iter(_root.Handle, out _iterHandle);
            }
        }
    }
}
