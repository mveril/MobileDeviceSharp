using System;
using System.Globalization;
#if !NET6_0
using TimeZoneConverter;
#endif
using System.Net.NetworkInformation;
using PlistSharp;
using IOSLib.Native;

    namespace IOSLib
    {
        public class IDevice : IDisposable
        {
            private const int USBMUXCONNECTIONTYPE_All = 3;
            public IDevice(string udid)
            {
                var ex = Native.IDevice.idevice_new_with_options(out var deviceHandle, udid, USBMUXCONNECTIONTYPE_All).GetException();
                if (ex != null)
                {
                    throw ex;
                }
                Handle = deviceHandle;
            }

            public IDevice(string udid, UsbmuxConnectionType connectionType)
            {
            var ex = Native.IDevice.idevice_new_with_options(out var deviceHandle, udid, (int)connectionType).GetException();
            if (ex != null)
            {
                throw ex;
            }
            Handle = deviceHandle;
            }

            public IDeviceHandle Handle { get; }

            public string Name
            {
                get
                {
                    using var lockdown = new Lockdown(this);
                    return lockdown.DeviceName;
                }
            }

        public string Udid
        {
            get
            {
                using var lockdown = new Lockdown(this);
                return lockdown.DeviceUdid;
            }
        }


        //public void Shutdown()
        //{
        //    Lockdown.lockdownd_client_new_with_handshake(Handle, out var lockdownClient, null);
        //    DiagnosticsRelay.diagnostics_relay_client_start_service(Handle, out var diagnosticsRelay, null);
        //    DiagnosticsRelay.diagnostics_relay_shutdown(diagnosticsRelay, DiagnosticsRelayAction.ActionFlagDisplayFail);
        //}

        //public void Reboot()
        //{
        //    Lockdown.lockdownd_client_new_with_handshake(Handle, out var lockdownClient, null);
        //    DiagnosticsRelay.diagnostics_relay_client_start_service(Handle, out var diagnosticsRelay, null);
        //    DiagnosticsRelay.diagnostics_relay_restart(diagnosticsRelay, DiagnosticsRelay.DiagnosticsRelayAction.ActionFlagDisplayFail);
        //}

        private LockdownError TryGetLockdownValue(string? domain, string key, out PlistNode plistNode)
        {
            using (var lockdown = new Lockdown(this))
            {
                return lockdown.TryGetValue(domain, key, out plistNode);
            }
        }
        private LockdownError TryGetLockdownValue(string key, out PlistNode? plistNode)
        {
            using (var lockdown = new Lockdown(this))
            {
                return lockdown.TryGetValue(key, out plistNode);
            }
        }
        private PlistNode GetLockdownValue(string? domain, string key)
        {
            using (var lockdown = new Lockdown(this))
            {
                return lockdown.GetValue(domain, key);
            }
        }

        private PlistNode GetLockdownValue(string key)
        {
            using (var lockdown = new Lockdown(this))
            {
                return lockdown.GetValue(key);
            }
        }

            public void Dispose()
            {
                ((IDisposable)Handle).Dispose();
            }

            public OSVersion OSVersion
            {
                get
                {
                    return OSVersion.FromDevice(this);
                }
            }
            public Battery Battery
            {
                get
                {
                    return new Battery(this);
                }
            }
            public DeviceClass DeviceClass
            {
                get
                {
                    using var deviceClassNode = (PlistString)GetLockdownValue("DeviceClass");
                    return (DeviceClass)Enum.Parse(typeof(DeviceClass), deviceClassNode.Value);
                }
            }

            public string ProductType
            {
                get
                {
                    using var pValue = (PlistString)GetLockdownValue("ProductType");
                    return pValue.Value;
                }
            }
            public string ModelDisplayName
            {
                get
                {
                    return this.ProductType switch
                    {
                        "iPhone13,1" => "iPhone 12 mini",
                        "iPhone13,2" => "iPhone 12",
                        "iPhone13,3" => "iPhone 12 Pro",
                        "iPhone13,4" => "iPhone 12 Pro Max",
                        "iPhone12,8" => "iPhone SE (2nd generation)",
                        "iPhone12,5" => "iPhone 11 Pro Max",
                        "iPhone12,3" => "iPhone 11 Pro",
                        "iPhone12,1" => "iPhone 11",
                        "iPhone11,2" => "iPhone XS",
                        "iPhone11,4" or "iPhone11,6" => "iPhone XS Max",
                        "iPhone11,8" => "iPhone XR",
                        "iPhone10,3" or "iPhone10,6" => "iPhone X",
                        "iPhone10,2" or "iPhone10,5" => "iPhone 8 Plus",
                        "iPhone10,1" or "iPhone10,4" => "iPhone 8",
                        "iPhone9,2" or "iPhone9,4" => "iPhone 7 Plus",
                        "iPhone9,1" or "iPhone9,3" => "iPhone 7",
                        "iPhone8,4" => "iPhone SE",
                        "iPhone8,2" => "iPhone 6S Plus",
                        "iPhone8,1" => "iPhone 6S",
                        "iPhone7,1" => "iPhone 6 Plus",
                        "iPhone7,2" => "iPhone 6",
                        "iPhone6,2" => "iPhone 5S Global",
                        "iPhone6,1" => "iPhone 5S GSM",
                        "iPhone5,4" => "iPhone 5C Global",
                        "iPhone5,3" => "iPhone 5C GSM",
                        "iPhone5,2" => "iPhone 5 Global",
                        "iPhone5,1" => "iPhone 5 GSM",
                        "iPhone4,1" => "iPhone 4S",
                        "iPhone3,3" => "iPhone 4 CDMA",
                        "iPhone3,1" or "iPhone3,2" => "iPhone 4 GSM",
                        "iPhone2,1" => "iPhone 3GS",
                        "iPhone1,2" => "iPhone 3G",
                        "iPhone1,1" => "iPhone",
                        "iPod9,1" => "iPod touch 7G",
                        "iPod7,1" => "iPod touch 6G",
                        "iPod5,1" => "iPod touch 5G",
                        "iPod4,1" => "iPod touch 4G",
                        "iPod3,1" => "iPod touch 3G",
                        "iPod2,1" => "iPod touch 2G",
                        "iPod1,1" => "iPod touch",
                        "iPad13,2" => "iPad Air (4th generation) Wi-Fi + Cellular",
                        "iPad13,1" => "iPad Air (4th generation) Wi-Fi",
                        "iPad11,7" => "iPad (8th Generation) Wi-Fi + Cellular",
                        "iPad11,6" => "iPad (8th Generation) Wi-Fi",
                        "iPad11,4" => "iPad Air (3rd generation) Wi-Fi + Cellular",
                        "iPad11,3" => "iPad Air (3rd generation) Wi-Fi",
                        "iPad11,2" => "iPad mini (5th generation) Wi-Fi + Cellular",
                        "iPad11,1" => "iPad mini (5th generation) Wi-Fi",
                        "iPad8,12" => "iPad Pro (12.9-inch) (4th generation) Wi-Fi + Cellular",
                        "iPad8,11" => "iPad Pro (12.9-inch) (4th generation) Wi-Fi",
                        "iPad8,10" => "iPad Pro (11-inch) (2nd generation) Wi-Fi + Cellular",
                        "iPad8,9" => "iPad Pro (11-inch) (2nd generation) Wi-Fi",
                        "iPad8,8" => "iPad Pro 12.9-inch (3rd Generation)",
                        "iPad8,7" => "iPad Pro 12.9-inch (3rd generation) Wi-Fi + Cellular",
                        "iPad8,6" => "iPad Pro 12.9-inch (3rd Generation)",
                        "iPad8,5" => "iPad Pro 12.9-inch (3rd Generation)",
                        "iPad8,4" => "iPad Pro 11-inch",
                        "iPad8,3" => "iPad Pro 11-inch Wi-Fi + Cellular",
                        "iPad8,2" => "iPad Pro 11-inch",
                        "iPad8,1" => "iPad Pro 11-inch Wi-Fi",
                        "iPad7,12" => "iPad (7th generation) Wi-Fi + Cellular",
                        "iPad7,11" => "iPad (7th generation) Wi-Fi",
                        "iPad7,6" => "iPad (6th generation) Wi-Fi + Cellular",
                        "iPad7,5" => "iPad (6th generation) Wi-Fi",
                        "iPad7,4" => "iPad Pro (10.5-inch) Wi-Fi + Cellular",
                        "iPad7,3" => "iPad Pro (10.5-inch) Wi-Fi",
                        "iPad7,2" => "iPad Pro 12.9-inch (2nd generation) Wi-Fi + Cellular",
                        "iPad7,1" => "iPad Pro 12.9-inch (2nd generation) Wi-Fi",
                        "iPad6,12" => "iPad (5th generation) Wi-Fi + Cellular",
                        "iPad6,11" => "iPad (5th generation) Wi-Fi",
                        "iPad6,8" => "iPad Pro 12.9-inch Wi-Fi + Cellular",
                        "iPad6,7" => "iPad Pro 12.9-inch Wi-Fi",
                        "iPad6,4" => "iPad Pro (9.7-inch) Wi-Fi + Cellular",
                        "iPad6,3" => "iPad Pro (9.7-inch) Wi-Fi",
                        "iPad5,4" => "iPad Air 2 Wi-Fi + Cellular",
                        "iPad5,3" => "iPad Air 2 Wi-Fi",
                        "iPad5,2" => "iPad mini 4 Wi-Fi + Cellular",
                        "iPad5,1" => "iPad mini 4 Wi-Fi",
                        "iPad4,9" => "iPad mini 3 Wi-Fi + Cellular (TD-LTE)",
                        "iPad4,8" => "iPad mini 3 Wi-Fi + Cellular",
                        "iPad4,7" => "iPad mini 3 Wi-Fi",
                        "iPad4,6" => "iPad mini 2 Wi-Fi + Cellular (TD-LTE)",
                        "iPad4,5" => "iPad mini 2 Wi-Fi + Cellular",
                        "iPad4,4" => "iPad mini 2 Wi-Fi",
                        "iPad4,3" => "iPad Air Wi-Fi + Cellular (TD-LTE)",
                        "iPad4,2" => "iPad Air Wi-Fi + Cellular",
                        "iPad4,1" => "iPad Air Wi-Fi",
                        "iPad3,6" => "iPad (4th generation) Wi-Fi + Cellular (MM)",
                        "iPad3,5" => "iPad (4th generation) Wi-Fi + Cellular",
                        "iPad3,4" => "iPad (4th generation) Wi-Fi",
                        "iPad3,3" => "iPad 3 Wi-Fi + Cellular (CDMA)",
                        "iPad3,2" => "iPad 3 Wi-Fi + Cellular (GSM)",
                        "iPad3,1" => "iPad 3 Wi-Fi",
                        "iPad2,7" => "iPad mini Wi-Fi + Cellular (MM)",
                        "iPad2,6" => "iPad mini Wi-Fi + Cellular",
                        "iPad2,5" => "iPad mini Wi-Fi",
                        "iPad2,4" => "iPad 2 Wi-Fi",
                        "iPad2,3" => "iPad 2 CDMA",
                        "iPad2,2" => "iPad 2 GSM",
                        "iPad2,1" => "iPad 2 Wi-Fi",
                        "iPad1,1" => "iPad",
                        _ => this.ProductType,
                    };
                }
            }
            public string SerialNumber
            {
                get
                {

                    using var pValue = (PlistString)GetLockdownValue("SerialNumber");
                    return pValue.Value;
                }
            }

            public CultureInfo Language
            {
                get
                {
                    using var pValue = (PlistString)GetLockdownValue("com.apple.international", "Language");
                    return new CultureInfo(pValue.Value);
                }
            }

            public RegionInfo Locale
            {
                get
                {
                    using var pValue = (PlistString)GetLockdownValue("com.apple.international", "Locale");
                    return new RegionInfo(pValue.Value);
                }
            }


            public ulong Capacity
            {
                get
                {
                    using var pValue = (PlistInteger)GetLockdownValue("com.apple.disk_usage", "TotalDiskCapacity");
                    return pValue.Value;
                }
            }

            public string PhoneNumber
            {
                get
                {
                    using var pValue = (PlistString)GetLockdownValue(nameof(PhoneNumber));
                    return pValue.Value;
                }
            }


            public bool HasTelephonyCapability
            {
                get
                {
                    using var pValue = (PlistBoolean)GetLockdownValue(nameof(HasTelephonyCapability));
                    return pValue.Value;
                }
            }
            public PhysicalAddress WiFiAddress
            {
                get
                {
                    using var pValue = (PlistString)GetLockdownValue(nameof(WiFiAddress));
                    return PhysicalAddress.Parse(pValue.Value.Replace(":", string.Empty).ToUpperInvariant());
                }
            }

            public PhysicalAddress EthernetAddress
            {
                get
                {
                    using var pValue = (PlistString)GetLockdownValue(nameof(EthernetAddress));
                    return PhysicalAddress.Parse(pValue.Value.Replace(":", string.Empty).ToUpperInvariant());
                }
            }

            public PhysicalAddress BluetoothAddress
            {
                get
                {
                    using var pValue = (PlistString)GetLockdownValue(nameof(BluetoothAddress));
                    return PhysicalAddress.Parse(pValue.Value.Replace(":", string.Empty).ToUpperInvariant());
                }
            }


            public TimeZoneInfo TimeZone
            {
                get
                {

                    using var pValue = (PlistString)GetLockdownValue(nameof(TimeZone));
#if NET6_0
                    return TimeZoneInfo.FindSystemTimeZoneById(pValue.Value);
#else
                    return TZConvert.GetTimeZoneInfo(pValue.Value);
#endif
                }
            }
            public string CPUArchitecture
            {
                get
                {
                    using var pValue = (PlistString)GetLockdownValue(nameof(CPUArchitecture));
                    return pValue.Value;
                }
            }
        }
    }