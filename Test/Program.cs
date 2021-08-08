using System;
using System.Diagnostics;
using System.IO;
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
                using var ld = new LockdownSession(device);
                try
                {
                    Console.WriteLine(await ld.PairAsync());
                }
                catch (LockdownException ex) when (ex.ErrorCode == (int)IOSLib.Native.LockdownError.PasswordProtected)
                {
                    Console.ReadLine();
                    Console.WriteLine(await ld.PairAsync());
                }
                using var session = new AFCSession(device);
                ProcessItem(session.Root);

            }
        }
        private static void ProcessLine(AFCDirectory current, string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                if (current.Parent!= null)
                {
                    ProcessItem(current.Parent);
                }
                else
                {
                    ProcessItem(current);
                }
            }
            else
            {
                ProcessItem(current.GetItem(line));
            }
        }

        private static void ShowFileInExplorer(string filePath)
        {
            try
            {
                var winDir = Environment.GetEnvironmentVariable("windir");
                if (winDir != null)
                {
                    var explorerPath = Path.Combine(winDir, @"explorer.exe");
                    var arguments = String.Format("/select, {0}{1}{0}", (char)34, filePath);
                    Process.Start(explorerPath, arguments);
                }
            }
            catch (Exception ex)
            {
                //handle the exception your way!
            }
        }

        private static void ProcessItem(AFCDirectory Dir)
        {
            foreach (var item in Dir.GetItems())
            {
                Console.WriteLine(item.Path);
                Console.WriteLine($"> {item.CreationTime}");
                Console.WriteLine($"> {item.LastModifiedTime}");
                Console.WriteLine($"> {item.Name}");
                Console.WriteLine($"> {item.Extension}");
            }
            ProcessLine(Dir,Console.ReadLine());
        }

        private static void ProcessItem(AFCFile file)
        {
            var p = Path.Combine(Path.GetTempPath(), file.Name);
            var outp = File.OpenWrite(p);
            var inp = file.OpenRead();
            inp.CopyTo(outp);
            inp.Close();
            outp.Close();
            ShowFileInExplorer(p);
            ProcessItem(file.Parent);
        }
        private static void ProcessItem(AFCItem item)
        {
            if (item.GetType() == typeof(AFCFile))
            {
                ProcessItem((AFCFile)item);
            }
            else if (item.GetType() == typeof(AFCDirectory))
            {
                ProcessItem((AFCDirectory)item);
            }
        }
    }
}
