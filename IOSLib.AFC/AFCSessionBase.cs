﻿using IOSLib.AFC.Native;
using IOSLib.Native;
using System;
using System.Collections.Generic;
using static IOSLib.AFC.Native.AFC;

namespace IOSLib.AFC
{
    public abstract class AFCSessionBase : ServiceSessionBase<AFCClientHandle>
    {
        protected AFCSessionBase(IDevice device, string serviceID) : base(device, serviceID, true)
        {

        }

        protected AFCSessionBase(IDevice device) : base(device)
        {

        }

        public abstract string Root { get; }

        protected override AFCClientHandle Init(LockdownServiceDescriptorHandle Descriptor)
        {
            var ex = afc_client_new(Device.Handle, Descriptor, out var handle).GetException();
            if (ex != null)
            {
                throw ex;
            }
            return handle;
        }

        protected override AFCClientHandle Init()
        {
            var ex = afc_client_start_service(Device.Handle, out var handle, null).GetException();
            if (ex != null)
            {
                throw ex;
            }
            return handle;
        }

        internal AFCItemType GetItemType(string path)
        {
            return AFCItemType.Create(GetFileInfo(path)["st_ifmt"]);
        }

        internal IReadOnlyDictionary<string,string> GetFileInfo(string path)
        {
            var ex = afc_get_file_info(Handle, path, out var col).GetException();
            if (ex != null)
                throw ex;
            return col;
        }


        public void MakeLink(string target, string link, AFCLinkType linkType)
        {
            var ex = afc_make_link(Handle, linkType, target, link).GetException();
            if (ex != null)
                throw ex;
        }
        public void Move(string path, string to)
        {
            var ex = afc_rename_path(Handle, path, to).GetException();
            if (ex != null)
                throw ex;
        }
    }
}
