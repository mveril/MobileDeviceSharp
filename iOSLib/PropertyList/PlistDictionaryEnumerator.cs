using IOSLib.Native;
using IOSLib.PropertyList.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    public partial class PlistDictionary
    {

        private class Enumerator : IEnumerator<KeyValuePair<string, PlistNode>>
        {
            private readonly PlistDictionary _root;
            private PlistDictIterHandle _iter_handle;

            public Enumerator(PlistDictionary root)
            {
                _root = root;
                plist_dict_new_iter(_root.Handle, out _iter_handle);
            }

            private KeyValuePair<string, PlistNode> current;
            public KeyValuePair<string,PlistNode> Current => current;

            object IEnumerator.Current => current;

            public void Dispose()
            {
                _iter_handle.Dispose();
            }

            public bool MoveNext()
            {
                plist_dict_next_item(_root.Handle, _iter_handle,out var key, out var currentHandle);
                if (key == IntPtr.Zero)
                {
                    return false;
                }
                string dicKey = (string)UTF8Marshaler.GetInstance().MarshalNativeToManaged(key);
                if (dicKey == null)
                {
                    throw new NullReferenceException();
                }
                current = new KeyValuePair<string,PlistNode>(dicKey, PlistNode.From(currentHandle));
                Marshal.FreeHGlobal(key);
                return true;
            }

            public void Reset()
            {
                plist_dict_new_iter(_root.Handle, out _iter_handle);
            }
        }
    }
}
