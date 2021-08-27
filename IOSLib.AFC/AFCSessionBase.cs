﻿using IOSLib.AFC.Native;
using IOSLib.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static IOSLib.AFC.Native.AFC;

namespace IOSLib.AFC
{
    public abstract class AFCSessionBase : ServiceSessionBase<AFCClientHandle,AFCError>
    {
        private static readonly StartServiceCallback<AFCClientHandle, AFCError> startCallback = afc_client_start_service;

        private static readonly ClientNewCallback<AFCClientHandle, AFCError> clientNewCallback = afc_client_new;

        protected AFCSessionBase(IDevice device, string serviceID) : base(device, serviceID, true, clientNewCallback)
        {

        }

        protected AFCSessionBase(IDevice device) : base(device, startCallback)
        {

        }

        public abstract string RootPath { get; }

        public AFCDirectory Root => new AFCDirectory(this,"/");

        public AFCItemType GetItemType(string path)
        {
            return GetItemType(GetFileInfo(path));
        }

        internal AFCItemType GetItemType(IReadOnlyDictionary<string,string> fileInfo)
        {
            return AFCItemType.Create(fileInfo["st_ifmt"]);
        }

        internal IReadOnlyDictionary<string,string> GetFileInfo(string path)
        {
            try
            {
                var ex = afc_get_file_info(Handle, path, out var col).GetException();
                if (ex != null)
                    throw ex;
                return col;
            }
            catch (AFCException ex) when (ex.ErrorCode == (int)AFCError.ObjectNotFound)
            {
                return new ReadOnlyDictionary<string, string>(new Dictionary<string,string>());
            }
        }
    }
}