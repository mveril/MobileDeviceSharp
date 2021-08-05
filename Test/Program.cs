using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IOSLib;
using IOSLib.AFC;

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
                }
                var session = new AFCSession(device);
                foreach (var item in session.Root.EnumerateItems())
                {
                    Console.WriteLine(item.Path);
                }
            }
        }
    }
}
