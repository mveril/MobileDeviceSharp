﻿using IOSLib.CompilerServices;
using IOSLib.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IOSLib.PropertyList.Native
{
    static class Plist
    {
        public const string LibraryName = "plist";

        static Plist()
        {
            IOSLib.Native.LibraryResolver.EnsureRegistered();
        }

        /// <summary>
        /// Create a new root PlistHandle type #PLIST_DICT
        /// </summary>
        /// <returns>
        /// the created plist
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_new_dict", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_new_dict();

        /// <summary>
        /// Create a new root PlistHandle type #PLIST_ARRAY
        /// </summary>
        /// <returns>
        /// the created plist
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_new_array", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_new_array();

        /// <summary>
        /// Create a new PlistHandle type #PLIST_STRING
        /// </summary>
        /// <param name="val">
        /// the sting value, encoded in UTF8.
        /// </param>
        /// <returns>
        /// the created item
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_new_string", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_new_string([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))] string val);

        /// <summary>
        /// Create a new PlistHandle type #PLIST_BOOLEAN
        /// </summary>
        /// <param name="val">
        /// the boolean value, 0 is false, other values are true.
        /// </param>
        /// <returns>
        /// the created item
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_new_bool", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_new_bool([MarshalAs(UnmanagedType.U1)] bool val);

        /// <summary>
        /// Create a new PlistHandle type #PLIST_UINT
        /// </summary>
        /// <param name="val">
        /// the unsigned integer value
        /// </param>
        /// <returns>
        /// the created item
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_new_uint", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_new_uint(ulong val);

        /// <summary>
        /// Create a new PlistHandle type #PLIST_REAL
        /// </summary>
        /// <param name="val">
        /// the real value
        /// </param>
        /// <returns>
        /// the created item
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_new_real", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_new_real(double val);

        /// <summary>
        /// Create a new PlistHandle type #PLIST_DATA
        /// </summary>
        /// <param name="val">
        /// the binary buffer
        /// </param>
        /// <param name="length">
        /// the length of the buffer
        /// </param>
        /// <returns>
        /// the created item
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_new_data", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_new_data(byte[] val, ulong length);

        /// <summary>
        /// Create a new PlistHandle type #PLIST_DATA
        /// </summary>
        /// <param name="val">
        /// the binary buffer
        /// </param>
        /// <param name="length">
        /// the length of the buffer
        /// </param>
        /// <returns>
        /// the created item
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_new_data", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static unsafe extern PlistHandle plist_new_data(byte* val, ulong length);

        /// <summary>
        /// Create a new PlistHandle type #PLIST_DATE
        /// </summary>
        /// <param name="sec">
        /// the number of seconds since 01/01/2001
        /// </param>
        /// <param name="usec">
        /// the number of microseconds
        /// </param>
        /// <returns>
        /// the created item
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_new_date", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_new_date(int sec, int usec);

        /// <summary>
        /// Create a new PlistHandle type #PLIST_UID
        /// </summary>
        /// <param name="val">
        /// the unsigned integer value
        /// </param>
        /// <returns>
        /// the created item
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_new_uid", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_new_uid(ulong val);

        /// <summary>
        /// Destruct a PlistHandle node and all its children recursively
        /// </summary>
        /// <param name="plist">
        /// the plist to free
        /// </param>
        [GenerateHandle("Plist")]
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_free", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_free(System.IntPtr plist);

        /// <summary>
        /// Return a copy of passed node and it's children
        /// </summary>
        /// <param name="node">
        /// the plist to copy
        /// </param>
        /// <returns>
        /// copied plist
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_copy", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_copy(PlistHandle node);

        /// <summary>
        /// Get size of a #PLIST_ARRAY node.
        /// </summary>
        /// <param name="node">
        /// the node of type #PLIST_ARRAY
        /// </param>
        /// <returns>
        /// size of the #PLIST_ARRAY node
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_array_get_size", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern uint plist_array_get_size(PlistHandle node);

        /// <summary>
        /// Get the nth item in a #PLIST_ARRAY node.
        /// </summary>
        /// <param name="node">
        /// the node of type #PLIST_ARRAY
        /// </param>
        /// <param name="n">
        /// the index of the item to get. Range is [0, array_size[
        /// </param>
        /// <returns>
        /// the nth item or NULL if node is not of type #PLIST_ARRAY
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_array_get_item", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_array_get_item(PlistHandle node, uint n);

        /// <summary>
        /// Get the index of an item. item must be a member of a #PLIST_ARRAY node.
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <returns>
        /// the node index or UINT_MAX if node index can't be determined
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_array_get_item_index", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern uint plist_array_get_item_index(PlistHandle node);

        /// <summary>
        /// Set the nth item in a #PLIST_ARRAY node.
        /// The previous item at index n will be freed using #plist_free
        /// </summary>
        /// <param name="node">
        /// the node of type #PLIST_ARRAY
        /// </param>
        /// <param name="item">
        /// the new item at index n. The array is responsible for freeing item when it is no longer needed.
        /// </param>
        /// <param name="n">
        /// the index of the item to get. Range is [0, array_size[. Assert if n is not in range.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_array_set_item", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_array_set_item(PlistHandle node, PlistHandle item, uint n);

        /// <summary>
        /// Append a new item at the end of a #PLIST_ARRAY node.
        /// </summary>
        /// <param name="node">
        /// the node of type #PLIST_ARRAY
        /// </param>
        /// <param name="item">
        /// the new item. The array is responsible for freeing item when it is no longer needed.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_array_append_item", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_array_append_item(PlistHandle node, PlistHandle item);

        /// <summary>
        /// Insert a new item at position n in a #PLIST_ARRAY node.
        /// </summary>
        /// <param name="node">
        /// the node of type #PLIST_ARRAY
        /// </param>
        /// <param name="item">
        /// the new item to insert. The array is responsible for freeing item when it is no longer needed.
        /// </param>
        /// <param name="n">
        /// The position at which the node will be stored. Range is [0, array_size[. Assert if n is not in range.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_array_insert_item", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_array_insert_item(PlistHandle node, PlistHandle item, uint n);

        /// <summary>
        /// Remove an existing position in a #PLIST_ARRAY node.
        /// Removed position will be freed using #plist_free.
        /// </summary>
        /// <param name="node">
        /// the node of type #PLIST_ARRAY
        /// </param>
        /// <param name="n">
        /// The position to remove. Range is [0, array_size[. Assert if n is not in range.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_array_remove_item", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_array_remove_item(PlistHandle node, uint n);

        /// <summary>
        /// Remove a node that is a child node of a #PLIST_ARRAY node.
        /// node will be freed using #plist_free.
        /// </summary>
        /// <param name="node">
        /// The node to be removed from its #PLIST_ARRAY parent.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_array_item_remove", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_array_item_remove(PlistHandle node);

        /// <summary>
        /// Create an iterator of a #PLIST_ARRAY node.
        /// The allocated iterator should be freed with the standard free function.
        /// </summary>
        /// <param name="node">
        /// The node of type #PLIST_ARRAY
        /// </param>
        /// <param name="iter">
        /// Location to store the iterator for the array.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_array_new_iter", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_array_new_iter(PlistHandle node, out PlistArrayIterHandle iter);

        /// <summary>
        /// Increment iterator of a #PLIST_ARRAY node.
        /// </summary>
        /// <param name="node">
        /// The node of type #PLIST_ARRAY.
        /// </param>
        /// <param name="iter">
        /// Iterator of the array
        /// </param>
        /// <param name="item">
        /// Location to store the item. The caller must *not* free the
        /// returned item. Will be set to NULL when no more items are left
        /// to iterate.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_array_next_item", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_array_next_item(PlistHandle node, PlistArrayIterHandle iter, out PlistHandle item);

        /// <summary>
        /// Get size of a #PLIST_DICT node.
        /// </summary>
        /// <param name="node">
        /// the node of type #PLIST_DICT
        /// </param>
        /// <returns>
        /// size of the #PLIST_DICT node
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_dict_get_size", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern uint plist_dict_get_size(PlistHandle node);

        /// <summary>
        /// Create an iterator of a #PLIST_DICT node.
        /// The allocated iterator should be freed with the standard free function.
        /// </summary>
        /// <param name="node">
        /// The node of type #PLIST_DICT.
        /// </param>
        /// <param name="iter">
        /// Location to store the iterator for the dictionary.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_dict_new_iter", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_dict_new_iter(PlistHandle node, out PlistDictIterHandle iter);

        /// <summary>
        /// Increment iterator of a #PLIST_DICT node.
        /// </summary>
        /// <param name="node">
        /// The node of type #PLIST_DICT
        /// </param>
        /// <param name="iter">
        /// Iterator of the dictionary
        /// </param>
        /// <param name="key">
        /// Location to store the key, or NULL. The caller is responsible
        /// for freeing the the returned string.
        /// </param>
        /// <param name="val">
        /// Location to store the value, or NULL. The caller must *not*
        /// free the returned value. Will be set to NULL when no more
        /// key/value pairs are left to iterate.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_dict_next_item", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_dict_next_item(PlistHandle node, PlistDictIterHandle iter, out System.IntPtr key, out PlistHandle val);

        /// <summary>
        /// Get key associated key to an item. Item must be member of a dictionary.
        /// </summary>
        /// <param name="node">
        /// the item
        /// </param>
        /// <param name="key">
        /// a location to store the key. The caller is responsible for freeing the returned string.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_dict_get_item_key", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_dict_get_item_key(PlistHandle node, out System.IntPtr key);

        /// <summary>
        /// Get the nth item in a #PLIST_DICT node.
        /// </summary>
        /// <param name="node">
        /// the node of type #PLIST_DICT
        /// </param>
        /// <param name="key">
        /// the identifier of the item to get.
        /// </param>
        /// <returns>
        /// the item or NULL if node is not of type #PLIST_DICT. The caller should not free
        /// the returned node.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_dict_get_item", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_dict_get_item(PlistHandle node, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string key);

        /// <summary>
        /// Get key node associated to an item. Item must be member of a dictionary.
        /// </summary>
        /// <param name="node">
        /// the item
        /// </param>
        /// <returns>
        /// the key node of the given item, or NULL.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_dict_item_get_key", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_dict_item_get_key(PlistHandle node);

        /// <summary>
        /// Set item identified by key in a #PLIST_DICT node.
        /// The previous item identified by key will be freed using #plist_free.
        /// If there is no item for the given key a new item will be inserted.
        /// </summary>
        /// <param name="node">
        /// the node of type #PLIST_DICT
        /// </param>
        /// <param name="item">
        /// the new item associated to key
        /// </param>
        /// <param name="key">
        /// the identifier of the item to set.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_dict_set_item", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_dict_set_item(PlistHandle node, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string key, PlistHandle item);

        /// <summary>
        /// Insert a new item into a #PLIST_DICT node.
        /// </summary>
        /// <param name="node">
        /// the node of type #PLIST_DICT
        /// </param>
        /// <param name="item">
        /// the new item to insert
        /// </param>
        /// <param name="key">
        /// The identifier of the item to insert.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_dict_insert_item", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_dict_insert_item(PlistHandle node, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string key, PlistHandle item);

        /// <summary>
        /// Remove an existing position in a #PLIST_DICT node.
        /// Removed position will be freed using #plist_free
        /// </summary>
        /// <param name="node">
        /// the node of type #PLIST_DICT
        /// </param>
        /// <param name="key">
        /// The identifier of the item to remove. Assert if identifier is not present.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_dict_remove_item", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_dict_remove_item(PlistHandle node, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string key);

        /// <summary>
        /// Merge a dictionary into another. This will add all key/value pairs
        /// from the source dictionary to the target dictionary, overwriting
        /// any existing key/value pairs that are already present in target.
        /// </summary>
        /// <param name="target">
        /// pointer to an existing node of type #PLIST_DICT
        /// </param>
        /// <param name="source">
        /// node of type #PLIST_DICT that should be merged into target
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_dict_merge", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_dict_merge(out PlistHandle target, PlistHandle source);

        /// <summary>
        /// Get the parent of a node
        /// </summary>
        /// <param name="node">
        /// the parent (NULL if node is root)
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_get_parent", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_get_parent(PlistHandle node);

        /// <summary>
        /// Get the #PlistHandleype of a node.
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <returns>
        /// the type of the node
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_get_node_type", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistType plist_get_node_type(PlistHandle node);

        /// <summary>
        /// Get the value of a #PLIST_KEY node.
        /// This function does nothing if node is not of type #PLIST_KEY
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="val">
        /// a pointer to a C-string. This function allocates the memory,
        /// caller is responsible for freeing it.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_get_key_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_get_key_val(PlistHandle node, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))] out string val);

        /// <summary>
        /// Get the value of a #PLIST_STRING node.
        /// This function does nothing if node is not of type #PLIST_STRING
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="val">
        /// a pointer to a C-string. This function allocates the memory,
        /// caller is responsible for freeing it. Data is UTF-8 encoded.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_get_string_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_get_string_val(PlistHandle node, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))] out string val);

        /// <summary>
        /// Get a pointer to the buffer of a #PLIST_STRING node.
        /// </summary>
        /// <param name="node">
        /// The node
        /// </param>
        /// <param name="length">
        /// If non-NULL, will be set to the length of the string
        /// </param>
        /// <returns>
        /// Pointer to the NULL-terminated buffer.
        /// </returns>
        /// <remarks>
        /// DO NOT MODIFY the buffer. Mind that the buffer is only available
        /// until the plist node gets freed. Make a copy if needed.
        /// </remarks>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_get_string_ptr", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern System.IntPtr plist_get_string_ptr(PlistHandle node, ref ulong length);

        /// <summary>
        /// Get the value of a #PLIST_BOOLEAN node.
        /// This function does nothing if node is not of type #PLIST_BOOLEAN
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="val">
        /// a pointer to a uint8_t variable.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_get_bool_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_get_bool_val(PlistHandle node, out byte val);

        /// <summary>
        /// Get the value of a #PLIST_UINT node.
        /// This function does nothing if node is not of type #PLIST_UINT
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="val">
        /// a pointer to a uint64_t variable.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_get_uint_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_get_uint_val(PlistHandle node, out ulong val);

        /// <summary>
        /// Get the value of a #PLIST_REAL node.
        /// This function does nothing if node is not of type #PLIST_REAL
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="val">
        /// a pointer to a double variable.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_get_real_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_get_real_val(PlistHandle node, out double val);

        /// <summary>
        /// Get the value of a #PLIST_DATA node.
        /// This function does nothing if node is not of type #PLIST_DATA
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="val">
        /// a pointer to an unallocated char buffer. This function allocates the memory,
        /// caller is responsible for freeing it.
        /// </param>
        /// <param name="length">
        /// the length of the buffer
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_get_data_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_get_data_val(PlistHandle node, out byte[] val, out ulong length);

        /// <summary>
        /// Get a pointer to the data buffer of a #PLIST_DATA node.
        /// </summary>
        /// <param name="node">
        /// The node
        /// </param>
        /// <param name="length">
        /// Pointer to a uint64_t that will be set to the length of the buffer
        /// </param>
        /// <returns>
        /// Pointer to the buffer
        /// </returns>
        /// <remarks>
        /// DO NOT MODIFY the buffer. Mind that the buffer is only available
        /// until the plist node gets freed. Make a copy if needed.
        /// </remarks>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_get_data_ptr", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern System.IntPtr plist_get_data_ptr(PlistHandle node, ref ulong length);

        /// <summary>
        /// Get the value of a #PLIST_DATE node.
        /// This function does nothing if node is not of type #PLIST_DATE
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="sec">
        /// a pointer to an int32_t variable. Represents the number of seconds since 01/01/2001.
        /// </param>
        /// <param name="usec">
        /// a pointer to an int32_t variable. Represents the number of microseconds
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_get_date_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_get_date_val(PlistHandle node, out int sec, out int usec);

        /// <summary>
        /// Get the value of a #PLIST_UID node.
        /// This function does nothing if node is not of type #PLIST_UID
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="val">
        /// a pointer to a uint64_t variable.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_get_uid_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_get_uid_val(PlistHandle node, out ulong val);

        /// <summary>
        /// Set the value of a node.
        /// Forces type of node to #PLIST_KEY
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="val">
        /// the key value
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_set_key_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_set_key_val(PlistHandle node, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))] string val);

        /// <summary>
        /// Set the value of a node.
        /// Forces type of node to #PLIST_STRING
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="val">
        /// the string value. The string is copied when set and will be
        /// freed by the node.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_set_string_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_set_string_val(PlistHandle node, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.CustomMarshaler,MarshalTypeRef = typeof(UTF8Marshaler))] string val);

        /// <summary>
        /// Set the value of a node.
        /// Forces type of node to #PLIST_BOOLEAN
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="val">
        /// the boolean value
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_set_bool_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_set_bool_val(PlistHandle node, byte val);

        /// <summary>
        /// Set the value of a node.
        /// Forces type of node to #PLIST_UINT
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="val">
        /// the unsigned integer value
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_set_uint_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_set_uint_val(PlistHandle node, ulong val);

        /// <summary>
        /// Set the value of a node.
        /// Forces type of node to #PLIST_REAL
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="val">
        /// the real value
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_set_real_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_set_real_val(PlistHandle node, double val);

        /// <summary>
        /// Set the value of a node.
        /// Forces type of node to #PLIST_DATA
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="val">
        /// the binary buffer. The buffer is copied when set and will
        /// be freed by the node.
        /// </param>
        /// <param name="length">
        /// the length of the buffer
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_set_data_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_set_data_val(PlistHandle node, byte[] val, ulong length);

        /// <summary>
        /// Set the value of a node.
        /// Forces type of node to #PLIST_DATE
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="sec">
        /// the number of seconds since 01/01/2001
        /// </param>
        /// <param name="usec">
        /// the number of microseconds
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_set_date_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_set_date_val(PlistHandle node, int sec, int usec);

        /// <summary>
        /// Set the value of a node.
        /// Forces type of node to #PLIST_UID
        /// </summary>
        /// <param name="node">
        /// the node
        /// </param>
        /// <param name="val">
        /// the unsigned integer value
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_set_uid_val", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_set_uid_val(PlistHandle node, ulong val);

        /// <summary>
        /// Export the #PlistHandle structure to XML format.
        /// </summary>
        /// <param name="plist">
        /// the root node to export
        /// </param>
        /// <param name="plist_xml">
        /// a pointer to a C-string. This function allocates the memory,
        /// caller is responsible for freeing it. Data is UTF-8 encoded.
        /// </param>
        /// <param name="length">
        /// a pointer to an uint32_t variable. Represents the length of the allocated buffer.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_to_xml", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_to_xml(PlistHandle plist, out IntPtr plistXml, out uint length);

        /// <summary>
        /// Frees the memory allocated by plist_to_xml().
        /// </summary>
        /// <param name="plist_xml">
        /// The buffer allocated by plist_to_xml().
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_to_xml_free", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_to_xml_free(System.IntPtr plistXml);

        /// <summary>
        /// Export the #PlistHandle structure to binary format.
        /// </summary>
        /// <param name="plist">
        /// the root node to export
        /// </param>
        /// <param name="plist_bin">
        /// a pointer to a char* buffer. This function allocates the memory,
        /// caller is responsible for freeing it.
        /// </param>
        /// <param name="length">
        /// a pointer to an uint32_t variable. Represents the length of the allocated buffer.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_to_bin", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_to_bin(PlistHandle plist, out System.IntPtr plistBin, out uint length);

        /// <summary>
        /// Frees the memory allocated by plist_to_bin().
        /// </summary>
        /// <param name="plist_bin">
        /// The buffer allocated by plist_to_bin().
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_to_bin_free", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_to_bin_free(System.IntPtr plistBin);

        /// <summary>
        /// Import the #PlistHandle structure from XML format.
        /// </summary>
        /// <param name="plist_xml">
        /// a pointer to the xml buffer.
        /// </param>
        /// <param name="length">
        /// length of the buffer to read.
        /// </param>
        /// <param name="plist">
        /// a pointer to the imported plist.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_from_xml", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_from_xml([System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string plistXml, uint length, out PlistHandle plist);

        /// <summary>
        /// Import the #PlistHandle structure from binary format.
        /// </summary>
        /// <param name="plist_bin">
        /// a pointer to the xml buffer.
        /// </param>
        /// <param name="length">
        /// length of the buffer to read.
        /// </param>
        /// <param name="plist">
        /// a pointer to the imported plist.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_from_bin", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_from_bin(byte[] plistBin, uint length, out PlistHandle plist);

        /// <summary>
        /// Import the #PlistHandle structure from memory data.
        /// This method will look at the first bytes of plist_data
        /// to determine if plist_data contains a binary or XML plist.
        /// </summary>
        /// <param name="plist_data">
        /// a pointer to the memory buffer containing plist data.
        /// </param>
        /// <param name="length">
        /// length of the buffer to read.
        /// </param>
        /// <param name="plist">
        /// a pointer to the imported plist.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_from_memory", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void plist_from_memory(byte[] plistData, uint length, out PlistHandle plist);

        /// <summary>
        /// Test if in-memory plist data is binary or XML
        /// This method will look at the first bytes of plist_data
        /// to determine if plist_data contains a binary or XML plist.
        /// This method is not validating the whole memory buffer to check if the
        /// content is truly a plist, it's only using some heuristic on the first few
        /// bytes of plist_data.
        /// </summary>
        /// <param name="plist_data">
        /// a pointer to the memory buffer containing plist data.
        /// </param>
        /// <param name="length">
        /// length of the buffer to read.
        /// </param>
        /// <returns>
        /// 1 if the buffer is a binary plist, 0 otherwise.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_is_binary", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int plist_is_binary(byte[] plistData, uint length);

        /// <summary>
        /// Get a node from its path. Each path element depends on the associated father node type.
        /// For Dictionaries, var args are casted to const char*, for arrays, var args are caster to uint32_t
        /// Search is breath first order.
        /// </summary>
        /// <param name="plist">
        /// the node to access result from.
        /// </param>
        /// <param name="length">
        /// length of the path to access
        /// </param>
        /// <returns>
        /// the value to access.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_access_path", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_access_path(PlistHandle plist, uint length);

        /// <summary>
        /// Variadic version of #plist_access_path.
        /// </summary>
        /// <param name="plist">
        /// the node to access result from.
        /// </param>
        /// <param name="length">
        /// length of the path to access
        /// </param>
        /// <param name="v">
        /// list of array's index and dic'st key
        /// </param>
        /// <returns>
        /// the value to access.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_access_pathv", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern PlistHandle plist_access_pathv(PlistHandle plist, uint length, System.IntPtr v);

        /// <summary>
        /// Compare two node values
        /// </summary>
        /// <param name="node_l">
        /// left node to compare
        /// </param>
        /// <param name="node_r">
        /// rigth node to compare
        /// </param>
        /// <returns>
        /// TRUE is type and value match, FALSE otherwise.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_compare_node_value", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern sbyte plist_compare_node_value(PlistHandle nodeL, PlistHandle nodeR);

        /// <summary>
        /// Helper function to check the value of a PLIST_BOOL node.
        /// </summary>
        /// <param name="boolnode">
        /// node of type PLIST_BOOL
        /// </param>
        /// <returns>
        /// 1 if the boolean node has a value of TRUE, 0 if FALSE,
        /// or -1 if the node is not of type PLIST_BOOL
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_bool_val_is_true", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int plist_bool_val_is_true(PlistHandle boolnode);

        /// <summary>
        /// Helper function to compare the value of a PLIST_UINT node against
        /// a given value.
        /// </summary>
        /// <param name="uintnode">
        /// node of type PLIST_UINT
        /// </param>
        /// <param name="cmpval">
        /// value to compare against
        /// </param>
        /// <returns>
        /// 0 if the node's value and cmpval are equal,
        /// 1 if the node's value is greater than cmpval,
        /// or -1 if the node's value is less than cmpval.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_uint_val_compare", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int plist_uint_val_compare(PlistHandle uintnode, ulong cmpval);

        /// <summary>
        /// Helper function to compare the value of a PLIST_UID node against
        /// a given value.
        /// </summary>
        /// <param name="uidnode">
        /// node of type PLIST_UID
        /// </param>
        /// <param name="cmpval">
        /// value to compare against
        /// </param>
        /// <returns>
        /// 0 if the node's value and cmpval are equal,
        /// 1 if the node's value is greater than cmpval,
        /// or -1 if the node's value is less than cmpval.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_uid_val_compare", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int plist_uid_val_compare(PlistHandle uidnode, ulong cmpval);

        /// <summary>
        /// Helper function to compare the value of a PLIST_REAL node against
        /// a given value.
        /// </summary>
        /// <param name="realnode">
        /// node of type PLIST_REAL
        /// </param>
        /// <param name="cmpval">
        /// value to compare against
        /// </param>
        /// <returns>
        /// 0 if the node's value and cmpval are (almost) equal,
        /// 1 if the node's value is greater than cmpval,
        /// or -1 if the node's value is less than cmpval.
        /// </returns>
        /// <remarks>
        /// WARNING: Comparing floating point values can give inaccurate
        /// results because of the nature of floating point values on computer
        /// systems. While this function is designed to be as accurate as
        /// possible, please don't rely on it too much.
        /// </remarks>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_real_val_compare", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int plist_real_val_compare(PlistHandle realnode, double cmpval);

        /// <summary>
        /// Helper function to compare the value of a PLIST_DATE node against
        /// a given set of seconds and fraction of a second since epoch.
        /// </summary>
        /// <param name="datenode">
        /// node of type PLIST_DATE
        /// </param>
        /// <param name="cmpsec">
        /// number of seconds since epoch to compare against
        /// </param>
        /// <param name="cmpusec">
        /// fraction of a second in microseconds to compare against
        /// </param>
        /// <returns>
        /// 0 if the node's date is equal to the supplied values,
        /// 1 if the node's date is greater than the supplied values,
        /// or -1 if the node's date is less than the supplied values.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_date_val_compare", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int plist_date_val_compare(PlistHandle datenode, int cmpsec, int cmpusec);

        /// <summary>
        /// Helper function to compare the value of a PLIST_STRING node against
        /// a given value.
        /// This function basically behaves like strcmp.
        /// </summary>
        /// <param name="strnode">
        /// node of type PLIST_STRING
        /// </param>
        /// <param name="cmpval">
        /// value to compare against
        /// </param>
        /// <returns>
        /// 0 if the node's value and cmpval are equal,
        /// > 0 if the node's value is lexicographically greater than cmpval,
        /// or
        /// <
        /// 0 if the node's value is lexicographically less than cmpval.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_string_val_compare", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int plist_string_val_compare(PlistHandle strnode, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string cmpval);

        /// <summary>
        /// Helper function to compare the value of a PLIST_STRING node against
        /// a given value, while not comparing more than n characters.
        /// This function basically behaves like strncmp.
        /// </summary>
        /// <param name="strnode">
        /// node of type PLIST_STRING
        /// </param>
        /// <param name="cmpval">
        /// value to compare against
        /// </param>
        /// <param name="n">
        /// maximum number of characters to compare
        /// </param>
        /// <returns>
        /// 0 if the node's value and cmpval are equal,
        /// > 0 if the node's value is lexicographically greater than cmpval,
        /// or
        /// <
        /// 0 if the node's value is lexicographically less than cmpval.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_string_val_compare_with_size", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int plist_string_val_compare_with_size(PlistHandle strnode, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string cmpval, uint n);

        /// <summary>
        /// Helper function to match a given substring in the value of a
        /// PLIST_STRING node.
        /// </summary>
        /// <param name="strnode">
        /// node of type PLIST_STRING
        /// </param>
        /// <param name="substr">
        /// value to match
        /// </param>
        /// <returns>
        /// 1 if the node's value contains the given substring,
        /// or 0 if not.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_string_val_contains", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int plist_string_val_contains(PlistHandle strnode, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string substr);

        /// <summary>
        /// Helper function to compare the value of a PLIST_KEY node against
        /// a given value.
        /// This function basically behaves like strcmp.
        /// </summary>
        /// <param name="keynode">
        /// node of type PLIST_KEY
        /// </param>
        /// <param name="cmpval">
        /// value to compare against
        /// </param>
        /// <returns>
        /// 0 if the node's value and cmpval are equal,
        /// > 0 if the node's value is lexicographically greater than cmpval,
        /// or
        /// <
        /// 0 if the node's value is lexicographically less than cmpval.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_key_val_compare", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int plist_key_val_compare(PlistHandle keynode, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string cmpval);

        /// <summary>
        /// Helper function to compare the value of a PLIST_KEY node against
        /// a given value, while not comparing more than n characters.
        /// This function basically behaves like strncmp.
        /// </summary>
        /// <param name="keynode">
        /// node of type PLIST_KEY
        /// </param>
        /// <param name="cmpval">
        /// value to compare against
        /// </param>
        /// <param name="n">
        /// maximum number of characters to compare
        /// </param>
        /// <returns>
        /// 0 if the node's value and cmpval are equal,
        /// > 0 if the node's value is lexicographically greater than cmpval,
        /// or
        /// <
        /// 0 if the node's value is lexicographically less than cmpval.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_key_val_compare_with_size", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int plist_key_val_compare_with_size(PlistHandle keynode, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string cmpval, uint n);

        /// <summary>
        /// Helper function to match a given substring in the value of a
        /// PLIST_KEY node.
        /// </summary>
        /// <param name="keynode">
        /// node of type PLIST_KEY
        /// </param>
        /// <param name="substr">
        /// value to match
        /// </param>
        /// <returns>
        /// 1 if the node's value contains the given substring,
        /// or 0 if not.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_key_val_contains", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int plist_key_val_contains(PlistHandle keynode, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string substr);

        /// <summary>
        /// Helper function to compare the data of a PLIST_DATA node against
        /// a given blob and size.
        /// This function basically behaves like memcmp after making sure the
        /// size of the node's data value is equal to the size of cmpval (n),
        /// making this a "full match" comparison.
        /// </summary>
        /// <param name="datanode">
        /// node of type PLIST_DATA
        /// </param>
        /// <param name="cmpval">
        /// data blob to compare against
        /// </param>
        /// <param name="n">
        /// size of data blob passed in cmpval
        /// </param>
        /// <returns>
        /// 0 if the node's data blob and cmpval are equal,
        /// > 0 if the node's value is lexicographically greater than cmpval,
        /// or
        /// <
        /// 0 if the node's value is lexicographically less than cmpval.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_data_val_compare", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int plist_data_val_compare(PlistHandle datanode, ref char cmpval, uint n);

        /// <summary>
        /// Helper function to compare the data of a PLIST_DATA node against
        /// a given blob and size, while no more than n bytes are compared.
        /// This function basically behaves like memcmp after making sure the
        /// size of the node's data value is at least n, making this a
        /// "starts with" comparison.
        /// </summary>
        /// <param name="datanode">
        /// node of type PLIST_DATA
        /// </param>
        /// <param name="cmpval">
        /// data blob to compare against
        /// </param>
        /// <param name="n">
        /// size of data blob passed in cmpval
        /// </param>
        /// <returns>
        /// 0 if the node's value and cmpval are equal,
        /// > 0 if the node's value is lexicographically greater than cmpval,
        /// or
        /// <
        /// 0 if the node's value is lexicographically less than cmpval.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_data_val_compare_with_size", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int plist_data_val_compare_with_size(PlistHandle datanode, ref char cmpval, uint n);

        /// <summary>
        /// Helper function to match a given data blob within the value of a
        /// PLIST_DATA node.
        /// </summary>
        /// <param name="datanode">
        /// node of type PLIST_KEY
        /// </param>
        /// <param name="cmpval">
        /// data blob to match
        /// </param>
        /// <param name="n">
        /// size of data blob passed in cmpval
        /// </param>
        /// <returns>
        /// 1 if the node's value contains the given data blob
        /// or 0 if not.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(Plist.LibraryName, EntryPoint = "plist_data_val_contains", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int plist_data_val_contains(PlistHandle datanode, ref char cmpval, uint n);
    }
}
