using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.AFC;
using MobileDeviceSharp.AFC.Native;
using MobileDeviceSharp.InstallationProxy;
using Mono.Unix;
using static MobileDeviceSharp.HouseArrest.Native.HouseArrest;

namespace MobileDeviceSharp.HouseArrest
{
    /// <summary>
    /// Represent an Apple File Conduit session created by an <see cref="HouseArrestSession"/> and target an <see cref="Application"/> container..
    /// </summary>
    public class AFCHouseArrestSession : AFCSessionBase
    {
        
        internal AFCHouseArrestSession(HouseArrestSession parent) : base(parent.Device, GetHandle(parent))
        {
            Parent = parent;
        }

        /// <summary>
        /// Get directly an <see cref="AFCHouseArrestSession"/> by implicitly creating the <see cref="HouseArrestSession"/>.
        /// </summary>
        /// <param name="application">The targeted <see cref="Application"/>.</param>
        /// <param name="location">The <see cref="HouseArrestLocation"/> which be autorized.</param>
        /// <returns>The Apple File Conduit session.</returns>
        public static AFCHouseArrestSession CreateNew(Application application, HouseArrestLocation location)
        {
            var houseArrest = new HouseArrestSession(application, location);
            return houseArrest.AFCSession;
        }

        /// <summary>
        /// Get the parent <see cref="HouseArrestSession"/> which create this <see cref="AFCSession"/>.
        /// </summary>
        public HouseArrestSession Parent { get; }

        static AFCClientHandle GetHandle(HouseArrestSession houseArrestSession)
        {
            var hresult = afc_client_new_from_house_arrest_client(houseArrestSession.Handle, out var afcHandle);
            if(hresult.IsError())
                throw hresult.GetException();
            return afcHandle;
        }

        /// <summary>
        /// Get the root path (this path corespond to the <see cref="Application.ContainerPath"/> of the chosen application.
        /// </summary>
        public override string RootPath
        {
            get
            {
                return Parent.Applicaton.ContainerPath;
            }
        }

        /// <summary>
        /// Get the document folder (accessible whatether you chose <see cref="HouseArrestLocation.Container"/> or <see cref="HouseArrestLocation.Documents"/>.
        /// </summary>
        public AFCDirectory Documents => this.GetAFCDirectory("/Documents");
    }
}
