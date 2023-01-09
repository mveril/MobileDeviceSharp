using MobileDeviceSharp.AFC.Native;
using MobileDeviceSharp.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static MobileDeviceSharp.AFC.Native.AFC;

namespace MobileDeviceSharp.AFC
{
    public abstract class AFCSessionBase : ServiceSessionBase<AFCClientHandle,AFCError>
    {
        private static readonly StartServiceCallback<AFCClientHandle, AFCError> s_startCallback = afc_client_start_service;

        private static readonly ClientNewCallback<AFCClientHandle, AFCError> s_clientNewCallback = afc_client_new;

        protected AFCSessionBase(IDevice device, string serviceID) : base(device, serviceID, true, s_clientNewCallback)
        {

        }

        protected AFCSessionBase(IDevice device) : base(device, s_startCallback)
        {

        }

        public abstract string RootPath { get; }

        protected AFCDirectory GetAFCDirectory(string path) => new AFCDirectory(this, path);

        public AFCDirectory Root => GetAFCDirectory("/");

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
        internal IReadOnlyDictionary<string,string> GetFileInfo(string path)
        {
            try
            {
                var hresult = afc_get_file_info(Handle, path, out var col);
                if (hresult.IsError())
                    throw hresult.GetException();
                return col;
            }
            catch (AFCException ex) when (ex.ErrorCode == (int)AFCError.ObjectNotFound)
            {
#if NETCOREAPP1_0_OR_GREATER
                return System.Collections.Immutable.ImmutableDictionary<string, string>.Empty;
#else
                return s_readonlydic;
#endif
            }
        }
    }
}
