using System;
using System.Collections.Generic;
using MobileDeviceSharp;
using MobileDeviceSharp.Native;
using MobileDeviceSharp.PropertyList;

namespace MobileDeviceSharp
{
    /// <summary>
    /// Represent the battery information of the device.
    /// </summary>
    public sealed class Battery
    {
        private readonly IDevice _device;

        private const string BATTERY_LOCKDOWN_DOMAIN = "com.apple.mobile.battery";
        internal Battery(IDevice iDevice)
        {
            _device = iDevice;
        }

        /// <summary>
        /// Get the device battery state.
        /// </summary>
        public UIDeviceBatteryState BatteryState
        {
            get
            {
                PlistDictionary dict;
                using (var lockdown =new LockdownSession(_device))
                {
                    if(lockdown.TryGetDomain(BATTERY_LOCKDOWN_DOMAIN, out var domain))
                    {
                        dict = (PlistDictionary)domain;
                    }
                    else
                    {
                        return UIDeviceBatteryState.Unknown;
                    }
                }
                IReadOnlyDictionary<string, ValueTuple<bool, UIDeviceBatteryState>> statesDic
                = new Dictionary<string, ValueTuple<bool, UIDeviceBatteryState>>
                {
                    { "FullyCharged", (true,UIDeviceBatteryState.Full) },
                    { "BatteryIsCharging", (true,UIDeviceBatteryState.Charging) },
                    { "ExternalConnected", (false,UIDeviceBatteryState.Unplugged) }
                };
                foreach (var keyValue in statesDic)
                {
                    var key = keyValue.Key;
                    var (val, state) = keyValue.Value;
                    if (dict.TryGetValue(key, out var pValue))
                    {
                        var pbool = ((PlistBoolean)pValue).Value;
                        if (pbool == val)
                        {
                            return state;
                        }
                    }
                    else
                    {
                        return UIDeviceBatteryState.Unknown;
                    }
                }
                return UIDeviceBatteryState.Unknown;
            }
        }

        /// <summary>
        /// Get the device battery level.
        /// </summary>
        public float BatteryLevel
        {
            get
            {
                using var lockdown = new LockdownSession(_device);
                using var pValue = (PlistInteger)lockdown.GetDomain(BATTERY_LOCKDOWN_DOMAIN)["BatteryCurrentCapacity"];
                return pValue.Value/100;
            }
        }
    }
}
