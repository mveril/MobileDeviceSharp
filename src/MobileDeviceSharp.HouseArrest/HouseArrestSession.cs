using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp;
using MobileDeviceSharp.HouseArrest.Native;
using MobileDeviceSharp.Native;
using MobileDeviceSharp.PropertyList;
using MobileDeviceSharp.AFC.Native;
using static MobileDeviceSharp.HouseArrest.Native.HouseArrest;
using MobileDeviceSharp.InstallationProxy;

namespace MobileDeviceSharp.HouseArrest
{
    /// <summary>
    /// Represente a session of the <see href="https://docs.libimobiledevice.org/libimobiledevice/latest/house__arrest_8h.html">HouseArrests</see> service.
    /// </summary>
    public class HouseArrestSession : ServiceSessionBase<HouseArrestClientHandle, HouseArrestError>
    {
        private static StartServiceCallback<HouseArrestClientHandle,HouseArrestError> s_startServiceCallback = house_arrest_client_start_service;

        /// <summary>
        /// Initialize a new afc session for the target <paramref name="application"/> at the specific <paramref name="location"/> the Device
        /// </summary>
        /// <param name="application">The targeted <see cref="Application"/>.</param>
        /// <param name="location">The <see cref="HouseArrestLocation"/> which be autorized.</param>
        public HouseArrestSession(Application application, HouseArrestLocation location) : base(application.Device, s_startServiceCallback)
        {
            Applicaton = application;
            Location = location;
            _aFCHouseArrestSession = new(InitializeAFCSession, true);
        }

        private AFCHouseArrestSession InitializeAFCSession()
        {
            var hresult = house_arrest_send_command(Handle, "Vend" + Enum.GetName(typeof(HouseArrestLocation), Location), Applicaton.BundleID);
            if (hresult.IsError())
                throw hresult.GetException();
            hresult = house_arrest_get_result(Handle, out var plistHandle);
            if (hresult.IsError())
                throw hresult.GetException();
            using var resultDic = (PlistDictionary)PlistNode.From(plistHandle)!;
            if (resultDic.TryGetValue("Error", out var errorPlist))
            {
                throw new NotSupportedException(((PlistString)errorPlist).Value);
            }
            return new AFCHouseArrestSession(this);
        }

        private Lazy<AFCHouseArrestSession> _aFCHouseArrestSession;

        /// <summary>
        /// Get the Apple file Conduit session created by this <see cref="HouseArrestSession"/>.
        /// </summary>
        public AFCHouseArrestSession AFCSession => _aFCHouseArrestSession.Value;

        /// <summary>
        /// Get the target application.
        /// </summary>
        public Application Applicaton { get; }

        /// <summary>
        /// Get the application location targeted by this session.
        /// </summary>
        public HouseArrestLocation Location { get; }
    }
}
