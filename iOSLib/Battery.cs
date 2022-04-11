using System;
using System.Collections.Generic;
using IOSLib;
using IOSLib.Native;
using IOSLib.PropertyList;

namespace IOSLib
{
    /// <summary>
    /// A class used to get the battery info of the device.
    /// </summary>
    public class Battery
    {
        private readonly IDevice device;

        private const string BATTERY_LOCKDOWN_DOMAIN = "com.apple.mobile.battery";
        internal Battery(IDevice iDevice)
        {
            device = iDevice;
        }

        /// <summary>
        /// Get the device battery state.
        /// </summary>
        public UIDeviceBatteryState BatteryState
        {
            get
            {
                LockdownError battryDataError = default;
                PlistNode plist;
                using (var lockdown =new LockdownSession(device))
                {
                    battryDataError = lockdown.TryGetValues(BATTERY_LOCKDOWN_DOMAIN, out plist);
                }
                if (battryDataError.IsError())
                {
                    return UIDeviceBatteryState.Unknown;
                }
                var dict = (PlistDictionary)plist!;
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
                    var isValid = dict.TryGetValue(key, out var pValue);
                    if (!isValid)
                    {
                        pValue.Dispose();
                        plist!.Dispose();
                        return UIDeviceBatteryState.Unknown;
                    }
                    else
                    {
                        var pbool = ((PlistBoolean)pValue).Value;
                        if (pbool == val)
                        {
                            return state;
                        }
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
                using var lockdown = new LockdownSession(device);
                using var pValue = (PlistInteger)lockdown.GetValue(BATTERY_LOCKDOWN_DOMAIN, "BatteryCurrentCapacity");
                return pValue.Value/100;
            }
        }
    }
}
