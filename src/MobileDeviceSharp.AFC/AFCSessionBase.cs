using MobileDeviceSharp.AFC.Native;
using MobileDeviceSharp.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static MobileDeviceSharp.AFC.Native.AFC;

namespace MobileDeviceSharp.AFC
{
    /// <summary>
    /// Represent a session of the <see href="https://docs.libimobiledevice.org/libimobiledevice/latest/afc_8h.html">Apple File Conduit</see> service.
    /// </summary>
    public abstract class AFCSessionBase : ServiceSessionBase<AFCClientHandle,AFCError>
    {
        private static readonly StartServiceCallback<AFCClientHandle, AFCError> s_startCallback = afc_client_start_service;

        private static readonly ClientNewCallback<AFCClientHandle, AFCError> s_clientNewCallback = afc_client_new;

        /// <summary>
        /// Initializes a new instance of the <see cref="AFCSessionBase"/> class for the specified device and service ID.
        /// </summary>
        /// <param name="device">The device to connect to.</param>
        /// <param name="serviceID">The ID of the service to connect to.</param>
        protected AFCSessionBase(IDevice device, string serviceID) : base(device, serviceID, true, s_clientNewCallback)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AFCSessionBase"/> class for the specified device.
        /// </summary>
        /// <param name="device">The device to connect to.</param>
        protected AFCSessionBase(IDevice device) : base(device, s_startCallback)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AFCSessionBase"/> class for the specified device and handle.
        /// </summary>
        /// <param name="device">The device to connect to.</param>
        /// <param name="handle">The handle to the AFC client to use for this session.</param>
        protected AFCSessionBase(IDevice device, AFCClientHandle handle) : base(device, handle)
        {

        }

        /// <summary>
        /// Get the absolute path of the root directory of this session.
        /// </summary>
        public abstract string RootPath { get; }

        /// <summary>
        /// Get the directory at the specified <paramref name="path"/> relative to this AFC session.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>the diretory.</returns>
        protected AFCDirectory GetAFCDirectory(string path) => new AFCDirectory(this, path);

        /// <summary>
        /// Gets the root directory of the AFC file system.
        /// </summary>
        public AFCDirectory Root => GetAFCDirectory("/");

        /// <summary>
        /// Get the <see cref="AFCDriveInfo"/> for this object which allow to find some imformation for the drive.
        /// </summary>
        public AFCDriveInfo DriveInfo  => new AFCDriveInfo(this);

        /// <summary>
        /// Gets the type of the item at the specified path.
        /// </summary>
        /// <param name="path">The path to the item to get the type of.</param>
        /// <returns>The type of the item at the specified path.</returns>
        public AFCItemType GetItemType(string path)
        {
            return GetItemType(GetFileInfo(path));
        }

        internal static AFCItemType GetItemType(IReadOnlyDictionary<string,string> fileInfo)
        {
            return AFCItemType.Create(fileInfo["st_ifmt"]);
        }
#if !NETCOREAPP2_0_OR_GREATER
        private static ReadOnlyDictionary<string, string> s_readonlydic = new(new Dictionary<string, string>());
#endif

        internal IReadOnlyDictionary<string, string> GetFileInfo(string path)
        {
            var hresult = afc_get_file_info(Handle, path, out var col);
            if (hresult == AFCError.ObjectNotFound)
            {
#if NETCOREAPP1_0_OR_GREATER
                return System.Collections.Immutable.ImmutableDictionary<string, string>.Empty;
#else
                return s_readonlydic;
#endif
            }
            if (hresult.IsError())
            {
                throw hresult.GetException();
            }
            return col;
        }
    }
}
