using IOSLib.PropertyList.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    public partial class PlistArray
    {

        private class Enumerator : IEnumerator<PlistNode>
        {
            private readonly PlistArray _root;
            private PlistArrayIterHandle _iter_handle;

            public Enumerator(PlistArray root)
            {
                _root = root;
                plist_array_new_iter(_root.Handle, out _iter_handle);
            }

            private PlistNode current;
            public PlistNode Current => current;

            object IEnumerator.Current => current;

            public void Dispose()
            {
                _iter_handle.Dispose();
            }

            public bool MoveNext()
            {
                plist_array_next_item(_root.Handle, _iter_handle, out var currentHandle);
                var success = !currentHandle.IsInvalid;
                current = PlistNode.From(currentHandle);
                return success;
            }

            public void Reset()
            {
                plist_array_new_iter(_root.Handle, out _iter_handle);
            }
        }
    }
}
