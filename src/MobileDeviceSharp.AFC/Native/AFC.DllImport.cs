﻿#if !NET7_0_OR_GREATER
using MobileDeviceSharp.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileDeviceSharp.AFC.Native
{
    internal static partial class AFC
    {
        /// <summary>
        /// Makes a connection to the AFC service on the device.
        /// </summary>
        /// <param name="device">
        /// The device to connect to.
        /// </param>
        /// <param name="service">
        /// The service descriptor returned by lockdownd_start_service.
        /// </param>
        /// <param name="client">
        /// Pointer that will be set to a newly allocated afc_client_t
        /// upon successful return.
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success, AFC_E_INVALID_ARG if device or service is
        /// invalid, AFC_E_MUX_ERROR if the connection cannot be established,
        /// or AFC_E_NO_MEM if there is a memory allocation problem.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_client_new", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_client_new(IDeviceHandle device, LockdownServiceDescriptorHandle service, out AFCClientHandle client);

        /// <summary>
        /// Starts a new AFC service on the specified device and connects to it.
        /// </summary>
        /// <param name="device">
        /// The device to connect to.
        /// </param>
        /// <param name="client">
        /// Pointer that will point to a newly allocated afc_client_t upon
        /// successful return. Must be freed using afc_client_free() after use.
        /// </param>
        /// <param name="label">
        /// The label to use for communication. Usually the program name.
        /// Pass NULL to disable sending the label in requests to lockdownd.
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success, or an AFC_E_* error code otherwise.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_client_start_service", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_client_start_service(IDeviceHandle device, out AFCClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef =typeof(UTF8Marshaler))] string? label);

        /// <summary>
        /// Frees up an AFC client. If the connection was created by the client itself,
        /// the connection will be closed.
        /// </summary>
        /// <param name="client">
        /// The client to free.
        /// </param>
        [MobileDeviceSharp.CompilerServices.UsedForRelease<AFCClientHandle>]
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_client_free", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_client_free(System.IntPtr client);

        /// <summary>
        /// Get device information for a connected client. The device information
        /// returned is the device model as well as the free space, the total capacity
        /// and blocksize on the accessed disk partition.
        /// </summary>
        /// <param name="client">
        /// The client to get device info for.
        /// </param>
        /// <param name="deviceInformation">
        /// A char list of device information terminated by an
        /// empty string or NULL if there was an error. Free with
        /// afc_dictionary_free().
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_get_device_info", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_get_device_info(AFCClientHandle client, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AFCDictionaryMarshaler))] out IReadOnlyDictionary<string, string> deviceInformation);

        /// <summary>
        /// Gets a directory listing of the directory requested.
        /// </summary>
        /// <param name="client">
        /// The client to get a directory listing from.
        /// </param>
        /// <param name="path">
        /// The directory for listing. (must be a fully-qualified path)
        /// </param>
        /// <param name="directoryInformation">
        /// A char list of files in the directory
        /// terminated by an empty string or NULL if there was an error. Free with
        /// afc_dictionary_free().
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_read_directory", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_read_directory(AFCClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef =typeof(UTF8DecomposedMarshaler))] string path, [MarshalAs(UnmanagedType.CustomMarshaler,MarshalTypeRef =typeof(ArrayMarshaller<string,UTF8DecomposedMarshaler>))] out string[] directoryInformation);

        /// <summary>
        /// Gets information about a specific file.
        /// </summary>
        /// <param name="client">
        /// The client to use to get the information of the file.
        /// </param>
        /// <param name="path">
        /// The fully-qualified path to the file.
        /// </param>
        /// <param name="fileInformation">
        /// Pointer to a buffer that will be filled with a
        /// NULL-terminated list of strings with the file information. Set to NULL
        /// before calling this function. Free with afc_dictionary_free().
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_get_file_info", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_get_file_info(AFCClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef =typeof(UTF8DecomposedMarshaler))] string path, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AFCDictionaryMarshaler))] out IReadOnlyDictionary<string, string> fileInformation);

        /// <summary>
        /// Opens a file on the device.
        /// </summary>
        /// <param name="client">
        /// The client to use to open the file.
        /// </param>
        /// <param name="filename">
        /// The file to open. (must be a fully-qualified path)
        /// </param>
        /// <param name="fileMode">
        /// The mode to use to open the file.
        /// </param>
        /// <param name="handle">
        /// Pointer to a uint64_t that will hold the handle of the file
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_file_open", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_file_open(AFCClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef =typeof(UTF8Marshaler))] string filename, AFCFileMode fileMode, out ulong handle);

        /// <summary>
        /// Closes a file on the device.
        /// </summary>
        /// <param name="client">
        /// The client to close the file with.
        /// </param>
        /// <param name="handle">
        /// File handle of a previously opened file.
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_file_close", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_file_close(AFCClientHandle client, ulong handle);

        /// <summary>
        /// Locks or unlocks a file on the device.
        /// Makes use of flock on the device.
        /// </summary>
        /// <param name="client">
        /// The client to lock the file with.
        /// </param>
        /// <param name="handle">
        /// File handle of a previously opened file.
        /// </param>
        /// <param name="operation">
        /// the lock or unlock operation to perform, this is one of
        /// AFC_LOCK_SH (shared lock), AFC_LOCK_EX (exclusive lock), or
        /// AFC_LOCK_UN (unlock).
        /// </param>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_file_lock", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_file_lock(AFCClientHandle client, ulong handle, AFCLockOp operation);

        /// <summary>
        /// Attempts to the read the given number of bytes from the given file.
        /// </summary>
        /// <param name="client">
        /// The relevant AFC client
        /// </param>
        /// <param name="handle">
        /// File handle of a previously opened file
        /// </param>
        /// <param name="data">
        /// The pointer to the memory region to store the read data
        /// </param>
        /// <param name="length">
        /// The number of bytes to read
        /// </param>
        /// <param name="bytesRead">
        /// The number of bytes actually read.
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_file_read", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_file_read(AFCClientHandle client, ulong handle, byte[] data, uint length, out uint bytesRead);

        /// <summary>
        /// Attempts to the read the given number of bytes from the given file.
        /// </summary>
        /// <param name="client">
        /// The relevant AFC client
        /// </param>
        /// <param name="handle">
        /// File handle of a previously opened file
        /// </param>
        /// <param name="data">
        /// The pointer to the memory region to store the read data
        /// </param>
        /// <param name="length">
        /// The number of bytes to read
        /// </param>
        /// <param name="bytesRead">
        /// The number of bytes actually read.
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_file_read", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_file_read(AFCClientHandle client, ulong handle, [In, Out] ArrayWithOffset data, uint length, out uint bytesRead);

        /// <summary>
        /// Attempts to the read the given number of bytes from the given file.
        /// </summary>
        /// <param name="client">
        /// The relevant AFC client
        /// </param>
        /// <param name="handle">
        /// File handle of a previously opened file
        /// </param>
        /// <param name="data">
        /// The pointer to the memory region to store the read data
        /// </param>
        /// <param name="length">
        /// The number of bytes to read
        /// </param>
        /// <param name="bytesRead">
        /// The number of bytes actually read.
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_file_read", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern unsafe AFCError afc_file_read(AFCClientHandle client, ulong handle, byte* data, uint length, out uint bytesRead);

        /// <summary>
        /// Writes a given number of bytes to a file.
        /// </summary>
        /// <param name="client">
        /// The client to use to write to the file.
        /// </param>
        /// <param name="handle">
        /// File handle of previously opened file.
        /// </param>
        /// <param name="data">
        /// The data to write to the file.
        /// </param>
        /// <param name="length">
        /// How much data to write.
        /// </param>
        /// <param name="bytesWritten">
        /// The number of bytes actually written to the file.
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_file_write", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_file_write(AFCClientHandle client, ulong handle, byte[] data, uint length, out uint bytesWritten);

        /// <summary>
        /// Writes a given number of bytes to a file.
        /// </summary>
        /// <param name="client">
        /// The client to use to write to the file.
        /// </param>
        /// <param name="handle">
        /// File handle of previously opened file.
        /// </param>
        /// <param name="data">
        /// The data to write to the file.
        /// </param>
        /// <param name="length">
        /// How much data to write.
        /// </param>
        /// <param name="bytesWritten">
        /// The number of bytes actually written to the file.
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_file_write", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_file_write(AFCClientHandle client, ulong handle, [In, Out] ArrayWithOffset data, uint length, out uint bytesWritten);

        /// <summary>
        /// Writes a given number of bytes to a file.
        /// </summary>
        /// <param name="client">
        /// The client to use to write to the file.
        /// </param>
        /// <param name="handle">
        /// File handle of previously opened file.
        /// </param>
        /// <param name="data">
        /// The data to write to the file.
        /// </param>
        /// <param name="length">
        /// How much data to write.
        /// </param>
        /// <param name="bytesWritten">
        /// The number of bytes actually written to the file.
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_file_write", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern unsafe AFCError afc_file_write(AFCClientHandle client, ulong handle, byte* data, uint length, out uint bytesWritten);

        /// <summary>
        /// Seeks to a given position of a pre-opened file on the device.
        /// </summary>
        /// <param name="client">
        /// The client to use to seek to the position.
        /// </param>
        /// <param name="handle">
        /// File handle of a previously opened.
        /// </param>
        /// <param name="offset">
        /// Seek offset.
        /// </param>
        /// <param name="whence">
        /// Seeking direction, one of SEEK_SET, SEEK_CUR, or SEEK_END.
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_file_seek", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_file_seek(AFCClientHandle client, ulong handle, long offset, SeekWhence whence);

        /// <summary>
        /// Returns current position in a pre-opened file on the device.
        /// </summary>
        /// <param name="client">
        /// The client to use.
        /// </param>
        /// <param name="handle">
        /// File handle of a previously opened file.
        /// </param>
        /// <param name="position">
        /// Position in bytes of indicator
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_file_tell", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_file_tell(AFCClientHandle client, ulong handle, out ulong position);

        /// <summary>
        /// Sets the size of a file on the device.
        /// </summary>
        /// <param name="client">
        /// The client to use to set the file size.
        /// </param>
        /// <param name="handle">
        /// File handle of a previously opened file.
        /// </param>
        /// <param name="newsize">
        /// The size to set the file to.
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        /// <remarks>
        /// This function is more akin to ftruncate than truncate, and truncate
        /// calls would have to open the file before calling this, sadly.
        /// </remarks>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_file_truncate", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_file_truncate(AFCClientHandle client, ulong handle, ulong newsize);

        /// <summary>
        /// Deletes a file or directory.
        /// </summary>
        /// <param name="client">
        /// The client to use.
        /// </param>
        /// <param name="path">
        /// The path to delete. (must be a fully-qualified path)
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_remove_path", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_remove_path(AFCClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef =typeof(UTF8DecomposedMarshaler))] string path);

        /// <summary>
        /// Renames a file or directory on the device.
        /// </summary>
        /// <param name="client">
        /// The client to have rename.
        /// </param>
        /// <param name="from">
        /// The name to rename from. (must be a fully-qualified path)
        /// </param>
        /// <param name="to">
        /// The new name. (must also be a fully-qualified path)
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_rename_path", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_rename_path(AFCClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef =typeof(UTF8Marshaler))] string from, [System.Runtime.InteropServices.MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef =typeof(UTF8Marshaler))] string to);

        /// <summary>
        /// Creates a directory on the device.
        /// </summary>
        /// <param name="client">
        /// The client to use to make a directory.
        /// </param>
        /// <param name="path">
        /// The directory's path. (must be a fully-qualified path, I assume
        /// all other mkdir restrictions apply as well)
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_make_directory", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_make_directory(AFCClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef =typeof(UTF8DecomposedMarshaler))] string path);

        /// <summary>
        /// Sets the size of a file on the device without prior opening it.
        /// </summary>
        /// <param name="client">
        /// The client to use to set the file size.
        /// </param>
        /// <param name="path">
        /// The path of the file to be truncated.
        /// </param>
        /// <param name="newsize">
        /// The size to set the file to.
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_truncate", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_truncate(AFCClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef =typeof(UTF8DecomposedMarshaler))] string path, ulong newsize);

        /// <summary>
        /// Creates a hard link or symbolic link on the device.
        /// </summary>
        /// <param name="client">
        /// The client to use for making a link
        /// </param>
        /// <param name="linktype">
        /// 1 = hard link, 2 = symlink
        /// </param>
        /// <param name="target">
        /// The file to be linked.
        /// </param>
        /// <param name="linkname">
        /// The name of link.
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_make_link", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_make_link(AFCClientHandle client, AFCLinkType linktype, [System.Runtime.InteropServices.MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef =typeof(UTF8Marshaler))] string target, [System.Runtime.InteropServices.MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef =typeof(UTF8Marshaler))] string linkname);

        /// <summary>
        /// Sets the modification time of a file on the device.
        /// </summary>
        /// <param name="client">
        /// The client to use to set the file size.
        /// </param>
        /// <param name="path">
        /// Path of the file for which the modification time should be set.
        /// </param>
        /// <param name="mtime">
        /// The modification time to set in nanoseconds since epoch.
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_set_file_time", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_set_file_time(AFCClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef =typeof(UTF8DecomposedMarshaler))] string path, ulong mtime);

        /// <summary>
        /// Deletes a file or directory including possible contents.
        /// </summary>
        /// <param name="client">
        /// The client to use.
        /// </param>
        /// <param name="path">
        /// The path to delete. (must be a fully-qualified path)
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        /// <remarks>
        /// Only available in iOS 6 and later.
        /// </remarks>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_remove_path_and_contents", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_remove_path_and_contents(AFCClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef =typeof(UTF8DecomposedMarshaler))] string path);

        /// <summary>
        /// Get a specific key of the device info list for a client connection.
        /// Known key values are: Model, FSTotalBytes, FSFreeBytes and FSBlockSize.
        /// This is a helper function for afc_get_device_info().
        /// </summary>
        /// <param name="client">
        /// The client to get device info for.
        /// </param>
        /// <param name="key">
        /// The key to get the value of.
        /// </param>
        /// <param name="value">
        /// The value for the key if successful or NULL otherwise.
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_get_device_info_key", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_get_device_info_key(AFCClientHandle client, [System.Runtime.InteropServices.MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef =typeof(UTF8Marshaler))] string key, [System.Runtime.InteropServices.MarshalAsAttribute(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))] out string value);

        /// <summary>
        /// Frees up a char dictionary as returned by some AFC functions.
        /// </summary>
        /// <param name="dictionary">
        /// The char array terminated by an empty string.
        /// </param>
        /// <returns>
        /// AFC_E_SUCCESS on success or an AFC_E_* error value.
        /// </returns>
        [System.Runtime.InteropServices.DllImportAttribute(AFC.LibraryName, EntryPoint = "afc_dictionary_free", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern AFCError afc_dictionary_free(System.IntPtr dictionary);
    }
}
#endif
