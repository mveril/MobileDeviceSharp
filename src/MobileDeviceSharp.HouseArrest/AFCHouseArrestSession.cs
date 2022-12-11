using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.AFC;
using MobileDeviceSharp.AFC.Native;
using Mono.Unix;
using static MobileDeviceSharp.HouseArrest.Native.HouseArrest;

namespace MobileDeviceSharp.HouseArrest
{
    public class AFCHouseArrestSession : AFCSessionBase
    {
        
        internal AFCHouseArrestSession(HouseArrestSession parent) : base(parent.Device, GetHandle(parent))
        {
            Parent = parent;
        }

        public HouseArrestSession Parent { get; }

        static AFCClientHandle GetHandle(HouseArrestSession houseArrestSession)
        {
            var hresult = afc_client_new_from_house_arrest_client(houseArrestSession.Handle, out var afcHandle);
            if(hresult.IsError())
                throw hresult.GetException();
            return afcHandle;
        }

        public override string RootPath
        {
            get
            {

                return Parent.Location switch
                {
                    HouseArrestLocation.Container => Parent.Applicaton.ContainerPath,
                    HouseArrestLocation.Documents => UnixPath.Combine(Parent.Applicaton.ContainerPath, "Documents"),
                    _ => throw new NotSupportedException(),
                };
            }
        }
    }
}
