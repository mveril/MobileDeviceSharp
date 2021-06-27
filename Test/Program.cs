using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IOSLib;

namespace Test
{
    class Program
    {
        static  async Task Main(string[] args)
        {
            foreach (var device in IDevice.List())
            {
                var ld = new LockdownSession(device);
                try
                {
                    Console.WriteLine(await ld.PairAsync());
                }
                catch (LockdownException ex) when (ex.ErrorCode == (int)IOSLib.Native.LockdownError.PasswordProtected)
                {
                    Console.ReadLine();
                    Console.WriteLine(await ld.PairAsync());
                    throw;
                }
                Console.WriteLine(device.OSVersion.BuildNumber.Major);
                Console.WriteLine(device.OSVersion.BuildNumber.Minor);
                Console.WriteLine(device.OSVersion.BuildNumber.Build);
                Console.WriteLine(device.DeviceTime);
            }
        }
    }
}
