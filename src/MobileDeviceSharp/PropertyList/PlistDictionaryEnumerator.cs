using MobileDeviceSharp.Native;
using MobileDeviceSharp.PropertyList.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList
{
    public partial class PlistDictionary
    {

        private class Enumerator : IEnumerator<KeyValuePair<string, PlistNode>>
        {
            private readonly PlistDictionary _root;
            private PlistDictIterHandle _iterHandle;

            public Enumerator(PlistDictionary root)
            {
                _root = root;
                plist_dict_new_iter(_root.Handle, out _iterHandle);
            }

            private KeyValuePair<string, PlistNode> _current;
            public KeyValuePair<string,PlistNode> Current => _current;

            object IEnumerator.Current => _current;

            public void Dispose()
            {
                _iterHandle.Dispose();
            }

            public bool MoveNext()
            {
                plist_dict_next_item(_root.Handle, _iterHandle, out var key, out var currentHandle);
                if (key is null)
                {
                    return false;
                }
                _current = new KeyValuePair<string,PlistNode>(key, From(currentHandle));
                return true;
            }

            public void Reset()
            {
                _iterHandle.Dispose();
                plist_dict_new_iter(_root.Handle, out _iterHandle);
            }
        }
    }
}
