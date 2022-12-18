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
    public class HouseArrestSession : ServiceSessionBase<HouseArrestClientHandle, HouseArrestError>
    {
        private static StartServiceCallback<HouseArrestClientHandle,HouseArrestError> s_startServiceCallback = house_arrest_client_start_service;

        public HouseArrestSession(IDevice device, Application application, HouseArrestLocation location) : base(device, s_startServiceCallback)
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
        public AFCHouseArrestSession AFCSession => _aFCHouseArrestSession.Value;

        public Application Applicaton { get; }

        public HouseArrestLocation Location { get; }
    }
}
