using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
#if !NETCOREAPP3_0_OR_GREATER
using NativeLibraryLoader;
#endif

namespace IOSLib.Native
{
    internal static class LibraryResolver
    {
        static LibraryResolver()
        {
#if NETCOREAPP3_0_OR_GREATER
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
#else
            LoadMobileDeviceLibrary();
            LoadUsbmuxdLibrary();
#endif
        }

        public static void EnsureRegistered()
        {
            // Dummy call to trigger the static constructor
        }

        public static void EnsureRegistered(string LibraryName)
        {

        }

        private static bool TryLoad(string name, out IntPtr lib)
        {
#if NETCOREAPP3_0_OR_GREATER
            return NativeLibrary.TryLoad(name, Assembly.GetExecutingAssembly(), null, out lib);
#else
            lib = LibraryLoader.GetPlatformDefaultLoader().LoadNativeLibrary(name, new DllResolver());
            return lib != IntPtr.Zero;
#endif
        }

#if NETCOREAPP3_0_OR_GREATER
        private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            //if (libraryName == Plist.PlistNativeMethods.LibraryName)
            //{
            //    return LoadPlistLibrary();
            //}

            if (libraryName == Usbmuxd.LibraryName)
            {
                return LoadUsbmuxdLibrary();
            }

            if (libraryName == IDevice.LibraryName)
            {
                return LoadMobileDeviceLibrary();
            }

            if (libraryName == DeviceActivation.LibraryName)
            {
                return LoadDeviceActivationLibrary();
            }

            return IntPtr.Zero;
        }
#endif

        private static IntPtr LoadPlistLibrary()
        {
            IntPtr lib = IntPtr.Zero;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (TryLoad("libplist-2.0.so.3", out lib))
                {
                    return lib;
                }
                else if (TryLoad("libplist-2.0.so", out lib))
                {
                    return lib;
                }
                else if (TryLoad("libplist.so.3", out lib))
                {
                    return lib;
                }
                else if (TryLoad("libplist.so", out lib))
                {
                    return lib;
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (TryLoad("libplist-2.0.dylib", out lib))
                {
                    return lib;
                }
                else if (TryLoad("libplist.dylib", out lib))
                {
                    return lib;
                }
            }

            return IntPtr.Zero;
        }

        private static IntPtr LoadUsbmuxdLibrary()
        {
            IntPtr lib = IntPtr.Zero;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (TryLoad("libusbmuxd.so.6", out lib))
                {
                    return lib;
                }
                else if (TryLoad("libusbmuxd.so.4", out lib))
                {
                    // Not all symbols will be available in libusbmuxd.so.4
                    return lib;
                }
                else if (TryLoad("libusbmuxd.so", out lib))
                {
                    return lib;
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (TryLoad("libusbmuxd.dylib", out lib))
                {
                    return lib;
                }
            }



            return IntPtr.Zero;
        }

        private static IntPtr LoadMobileDeviceLibrary()
        {
            IntPtr lib = IntPtr.Zero;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (TryLoad("libimobiledevice-1.0.so.6", out lib))
                {
                    return lib;
                }
                else if (TryLoad("libimobiledevice-1.0.so", out lib))
                {
                    return lib;
                }
                else if (TryLoad("libimobiledevice.so.6", out lib))
                {
                    return lib;
                }
                else if (TryLoad("libimobiledevice.so", out lib))
                {
                    return lib;
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (TryLoad("libimobiledevice-1.0.dylib", out lib))
                {
                    return lib;
                }
                else if (TryLoad("libimobiledevice.dylib", out lib))
                {
                    return lib;
                }
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (TryLoad("libimobiledevice.dll", out lib))
                {
                    return lib;
                }
            }
            return IntPtr.Zero;
        }

        private static IntPtr LoadDeviceActivationLibrary()
        {
            IntPtr lib = IntPtr.Zero;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (TryLoad("libideviceactivation-1.0.so.2", out lib))
                {
                    return lib;
                }
                else if (TryLoad("libideviceactivation-1.0.so", out lib))
                {
                    return lib;
                }
                else if (TryLoad("libideviceactivation.so.2", out lib))
                {
                    return lib;
                }
                else if (TryLoad("libideviceactivation.so", out lib))
                {
                    return lib;
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (TryLoad("libideviceactivation-1.0.dylib", out lib))
                {
                    return lib;
                }
                else if (TryLoad("libideviceactivation.dylib", out lib))
                {
                    return lib;
                }
            }

            return IntPtr.Zero;
        }
    }
#if !NETCOREAPP3_0_OR_GREATER
    class DllResolver : PathResolver
    {
        public override IEnumerable<string> EnumeratePossibleLibraryLoadTargets(string name)
        {
            foreach (var item in PathResolver.Default.EnumeratePossibleLibraryLoadTargets(name))
            {
                yield return item;
            }
            string? path = null;
            try
            {
                path = System.IO.Path.Combine(getRuntimeIdentifier(), "native");
            }
            catch (Exception)
            {

            }

            if (path != null)
            {
                yield return path;
            }
        }

        private string getRuntimeIdentifier()
        {
            string? OS = null;
            string? archi = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                OS = "win";
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                OS = "osx";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "linux";
            }
            archi = RuntimeInformation.OSArchitecture.ToString().ToLower();
            if (OS== null)
            {
                throw new PlatformNotSupportedException();
            }
            return OS + "-" + archi;
        }
    }
#endif
}