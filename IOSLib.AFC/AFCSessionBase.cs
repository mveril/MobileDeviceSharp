using IOSLib.AFC.Native;
using IOSLib.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static IOSLib.AFC.Native.AFC;

namespace IOSLib.AFC
{
    public abstract class AFCSessionBase : ServiceSessionBase<AFCClientHandle,AFCError>
    {
        protected AFCSessionBase(IDevice device, string serviceID) : base(device, serviceID, true, new ClientNewCallback<AFCClientHandle, AFCError>(afc_client_new))
        {

        }

        protected AFCSessionBase(IDevice device) : base(device, new StartServiceCallback<AFCClientHandle, AFCError>(afc_client_start_service))
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

        public void Delete(string path)
        {
            var ex = afc_remove_path(Handle, path).GetException();
            if (ex != null)
                throw ex;
        }

        public void Delete(string path, bool recursive)
        {
            if (recursive)
            {
                var ex = afc_remove_path_and_contents(Handle, path).GetException();
                if (ex != null)
                    throw ex;
            }
            else
            {
                Delete(path);
            }
        }
    }
}