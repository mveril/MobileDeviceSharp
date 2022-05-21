using System;
using System.Globalization;
#if !NET6_0_OR_GREATER
using TimeZoneConverter;
#endif
using System.Net.NetworkInformation;
using IOSLib.Native;
using IOSLib.NotificationProxy.Native;
using IOSLib.Usbmuxd.Native;
using IOSLib.PropertyList;
using static IOSLib.Native.IDevice;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using IOSLib.CompilerServices;

namespace IOSLib
{
    /// <summary>
    /// Represent an Apple Device
    /// </summary>
    public partial class IDevice : IOSHandleWrapperBase<IDeviceHandle>
    {
        /// <summary>
        /// Initialize an apple device with his udid
        /// </summary>
        /// <param name="udid">The unique identifier</param>
        public IDevice(string udid) : base(GetHandle(udid))
        {

        }

        /// <summary>
        /// Initialize an apple device with his udid and the connection type
        /// </summary>
        /// <param name="udid"></param>
        /// <param name="connectionType"></param>
        public IDevice(string udid, IDeviceLookupOptions connectionType) : base(GetHandle(udid,connectionType))
        {

        }

        private IDevice(IDeviceHandle handle) : base(handle)
        {

        }

        private static IDeviceHandle GetHandle(string udid, IDeviceLookupOptions connectionType = IDeviceLookupOptions.All)
        {
            var hresult = idevice_new_with_options(out var deviceHandle, udid, connectionType);
            if (hresult.IsError())
                throw hresult.GetException();
            return deviceHandle;
        }

        internal static bool TryGetDevice(string udid, IDeviceLookupOptions connectionType, out IDevice device)
        {
            var result = idevice_new_with_options(out var deviceHandle, udid, connectionType) == 0;
            device = result ? new IDevice(deviceHandle) : null;
            return result;
        }

        internal static bool TryGetDevice(string udid, out IDevice device)
        {
            var result = idevice_new(out var deviceHandle, udid) == 0;
            device = result ? new IDevice(deviceHandle) : null;
            return result;
        }

        /// <summary>
        /// Get the list of all available devices
        /// </summary>
        /// <returns>Get the list of currently available devices</returns>
        public static IEnumerable<IDevice> List()
        {
            var hresult = idevice_get_device_list(out var udids, out _);
            if (hresult.IsError())
                throw hresult.GetException();
            return udids.Select(id => new IDevice(id));
        }

        /// <summary>
        /// An extended list of apple device with also device available via Wifi
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IDevice> ListExtended()
        {
            var hresult = idevice_get_device_list_extended(out var udids, out _);
            if (hresult.IsError())
                throw hresult.GetException();
            return udids.Select(id => new IDevice(id));
        }

        /// <summary>
        /// Occure when the device name changed
        /// </summary>
        [NotificationProxyEventName(NotificationProxyEvents.Recevable.NP_DEVICE_NAME_CHANGED)]
        public event EventHandler? NameChanged;

        /// <summary>
        /// Get the device name
        /// </summary>
        public string Name
        {
            get
            {
                using var lockdown = new LockdownSession(this,IsPaired);
                return lockdown.DeviceName;
            }
            set
            {
                using var lockdown = new LockdownSession(this, IsPaired);
                lockdown.DeviceName = value;
            }
        }

        /// <summary>
        /// Get the device Udid
        /// </summary>
        public string Udid
        {
            get
            {
                using var lockdown = new LockdownSession(this,IsPaired);
                return lockdown.DeviceUdid;
            }
        }
        /// <summary>
        /// Get the OS version information
        /// </summary>
        public OSVersion OSVersion
        {
            get
            {
                return OSVersion.FromDevice(this);
            }
        }

        /// <summary>
        /// Get battery information
        /// </summary>
        public Battery Battery
        {
            get
            {
                return new Battery(this);
            }
        }

        public string RawDeviceClass
        {
            get
            {
                using var lockdown = new LockdownSession(this, IsPaired);
                return lockdown.GetRawDeviceClass();
            }
        }

        /// <summary>
        /// Get the device class
        /// </summary>
        public DeviceClass DeviceClass
        {
            get
            {
                using var lockdown = new LockdownSession(this, IsPaired);
                return lockdown.GetDeviceClass();              
            }
        }

        /// <summary>
        /// Get the product type
        /// </summary>
        public string ProductType
        {
            get
            {
                using var lockdown = new LockdownSession(this,IsPaired);
                using var pValue = (PlistString)lockdown.GetDomain()["ProductType"];
                return pValue.Value;
            }
        }
        /// <summary>
        /// Get the model display name from the product type
        /// </summary>
        public string ModelDisplayName
        {
            get
            {
                return ProductType switch
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
                    _ => ProductType,
                };
            }
        }

        /// <summary>
        /// Get the serial number
        /// </summary>
        public string SerialNumber
        {
            get
            {
                using var lockdown = new LockdownSession(this,IsPaired);
                using var pValue = (PlistString)lockdown.GetDomain()["SerialNumber"];
                return pValue.Value;
            }
        }


        /// <summary>
        /// Occure when the language changed
        /// </summary>
        [NotificationProxyEventName(NotificationProxyEvents.Recevable.NP_LANGUAGE_CHANGED)]
        public event EventHandler? LanguageChanged;

        /// <summary>
        /// Get the <see cref="CultureInfo"/> representing the device language
        /// </summary>
        public CultureInfo Language
        {
            get
            {
                string strLang = string.Empty;
                using (var lockdown = new LockdownSession(this,IsPaired))
                {
                    using (PlistString deviceClassNode = (PlistString)lockdown.GetDomain("com.apple.international")["Language"])
                    {
                        strLang = deviceClassNode.Value;
                    }
                }
                return new CultureInfo(strLang);
            }
        }

        /// <summary>
        /// Get the <see cref="RegionInfo"/> representing the device locale
        /// </summary>
        public RegionInfo Locale
        {
            get
            {
                string strLocale = string.Empty;
                using (var lockdown = new LockdownSession(this,IsPaired))
                {
                    using (PlistString deviceLocale = (PlistString)lockdown.GetDomain("com.apple.international")["Locale"])
                    {
                        strLocale = deviceLocale.Value;
                    }
                }
                return new RegionInfo(strLocale.Replace('_','-'));
            }
        }

        /// <summary>
        /// Get the device capacity as byte
        /// </summary>
        public ulong Capacity
        {
            get
            {
                using var lockdown = new LockdownSession(this,IsPaired);
                using var pValue = (PlistInteger)lockdown.GetDomain("com.apple.disk_usage")["TotalDiskCapacity"];
                return pValue.Value;
            }
        }

        /// <summary>
        /// Get or set a value indicate if the wifi connection is enabled
        /// </summary>
        public bool WifiConnectionEnabled
        {
            get
            {
                using var lockdown = new LockdownSession(this, IsPaired);
                using var pValue = (PlistBoolean)lockdown.GetDomain("com.apple.mobile.wireless_lockdown")["EnableWifiConnections"];
                return pValue.Value;
            }
            set
            {
                using var lockdown = new LockdownSession(this, IsPaired);
                lockdown.GetDomain("com.apple.mobile.wireless_lockdown")["EnableWifiConnections"] = new PlistBoolean(value);
            }
        }

        /// <summary>
        /// Occure when <see cref="PhoneNumber"/> changed
        /// </summary>
        [NotificationProxyEventName(NotificationProxyEvents.Recevable.NP_PHONE_NUMBER_CHANGED)]
        public event EventHandler? PhoneNumberChanged;

        /// <summary>
        /// Get the Phone number of the device
        /// </summary>
        public string PhoneNumber
        {
            get
            {
                using var lockdown = new LockdownSession(this,IsPaired);
                using var pValue = (PlistString)lockdown.GetDomain()[nameof(PhoneNumber)];
                return pValue.Value;
            }
        }

        /// <summary>
        /// Check if the device has telephony capability
        /// </summary>
        public bool HasTelephonyCapability
        {
            get
            {
                using var lockdown = new LockdownSession(this,IsPaired);
                using var pValue = (PlistBoolean)lockdown.GetDomain()[nameof(HasTelephonyCapability)];
                return pValue.Value;
            }
        }

        /// <summary>
        /// Get the wifi adress of the device
        /// </summary>
        public PhysicalAddress WiFiAddress
        {
            get
            {
                string strAdress = string.Empty;
                using (var lockdown = new LockdownSession(this,IsPaired))
                {
                    using (PlistString deviceClassNode = (PlistString)lockdown.GetDomain()[nameof(WiFiAddress)])
                    {
                        strAdress = deviceClassNode.Value;
                    }
                }
                return ParsePhyAdress(strAdress);
            }
        }

        /// <summary>
        /// Get the Ethernet adress of the device
        /// </summary>
        public PhysicalAddress EthernetAddress
        {
            get
            {
                string strAdress = string.Empty;
                using (var lockdown = new LockdownSession(this,IsPaired))
                {
                    using (PlistString deviceClassNode = (PlistString)lockdown.GetDomain()[nameof(EthernetAddress)])
                    {
                        strAdress = deviceClassNode.Value;
                    }
                }
                return ParsePhyAdress(strAdress);
            }
        }

        private static PhysicalAddress ParsePhyAdress(string strAdress)
        {
            return PhysicalAddress.Parse(strAdress.Replace(":", string.Empty).ToUpperInvariant());
        }

        /// <summary>
        /// Get the Bluetooth adress of the device
        /// </summary>
        public PhysicalAddress BluetoothAddress
        {
            get
            {
                string strAdress = string.Empty;
                using (var lockdown = new LockdownSession(this,IsPaired))
                {
                    using (PlistString deviceClassNode = (PlistString)lockdown.GetDomain()[nameof(BluetoothAddress)])
                    {
                        strAdress = deviceClassNode.Value;
                    }
                }
                return ParsePhyAdress(strAdress);
            }
        }

        /// <summary>
        /// Occure when <see cref="TimeZone"/> changed
        /// </summary>
        [NotificationProxyEventName(NotificationProxyEvents.Recevable.NP_TIMEZONE_CHANGED)]
        public event EventHandler? TimeZoneChanged;
        public TimeZoneInfo TimeZone
        {
            get
            {
                string strTZ = string.Empty;
                using (var lockdown = new LockdownSession(this,IsPaired))
                {
                    using (PlistString timeZoneNode = (PlistString)lockdown.GetDomain()[nameof(TimeZone)])
                    {
                        strTZ = timeZoneNode.Value;
                    }
                }
#if !NET6_0_OR_GREATER
#if NET5_0
                if (OperatingSystem.IsWindows())
#else
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
#endif
                {
                    return TZConvert.GetTimeZoneInfo(strTZ);
                }
                else
                {
#endif
                    return TimeZoneInfo.FindSystemTimeZoneById(strTZ);
#if !NET6_0_OR_GREATER
                }
#endif
            }
            set
            {
                string strTZ;
#if !NET6_0_OR_GREATER
#if NET5_0
                if (OperatingSystem.IsWindows())
#else
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
#endif
                {
                    strTZ = TZConvert.WindowsToIana(value.Id);
                }
                else
                {

                    strTZ = value.Id;
                }
#else
                if (value.HasIanaId)
                {
                    strTZ = value.Id;
                }
                else
                {
                    if (TimeZoneInfo.TryConvertWindowsIdToIanaId(value.Id, out var strTZt))
                    {
                        strTZ = strTZt!;
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
#endif
                using (var lockdown = new LockdownSession(this, IsPaired))
                {
                    using var timeZoneNode = new PlistString(strTZ);
                    lockdown.GetDomain()[nameof(TimeZone)] = timeZoneNode;                   
                }
            }
        }

        /// <summary>
        /// Get the Date and time of the device
        /// </summary>
        public DateTimeOffset DeviceTime
        {
            get
            {
                double offset;
                using (
                    var lockdown = new LockdownSession(this,IsPaired))
                {
                    using (var intervalNode = (PlistReal)lockdown.GetDomain()["TimeIntervalSince1970"])
                    {
                        offset = intervalNode.Value;
                    }
                }
#if NETSTANDARD2_1_OR_GREATER  || NETCOREAPP2_1_OR_GREATER
                DateTimeOffset unix = DateTimeOffset.UnixEpoch;

#else
                DateTimeOffset unix = DateTimeOffset.FromUnixTimeSeconds(0);
#endif
                var utctime = unix.AddSeconds(offset);
                return TimeZoneInfo.ConvertTime(utctime, TimeZone);
            }
        }

        /// <summary>
        /// Get the CPU architecture of the device
        /// </summary>
        public string CPUArchitecture
        {
            get
            {
                using var lockdown = new LockdownSession(this,IsPaired);
                using var pValue = (PlistString)lockdown.GetDomain()[nameof(CPUArchitecture)];
                return pValue.Value;
            }
        }

        private bool _isPared;

        /// <summary>
        /// Check if the device is pared whith the computer
        /// </summary>
        public bool IsPaired { 
            get
            {
                return _isPared;
            }
            internal set {
                if (_isPared != value)
                {
                    _isPared = value;
                    if (value)
                    {
                        InitEventWatching(this);
                    }
                    else
                    {
                        CloseEventWatching();
                    }
                }
            }
        }
    }
}
