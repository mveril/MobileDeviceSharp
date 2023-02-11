using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Text;
using MobileDeviceSharp.AFC;

namespace SampleConsole
{
    internal class Explorer
    {
        public static void Start(AFCDirectory AFCDirectory, IConsole console)
        {
            ProcessItem(AFCDirectory, console);
        }

        private static void ProcessLine(AFCDirectory current, string line, IConsole console)
        {
            if (string.IsNullOrEmpty(line))
            {
                if (current.Parent is not null)
                {
                    ProcessItem(current.Parent, console);
                }
                else
                {
                    ProcessItem(current, console);
                }
            }
            else
            {
                ProcessItem(current.GetItem(line), console);
            }
        }

        private static void ShowFileInExplorer(string filePath)
        {
            try
            {
                var winDir = Environment.GetEnvironmentVariable("windir");
                if (winDir is not null)
                {
                    var explorerPath = Path.Combine(winDir, @"explorer.exe");
                    var arguments = string.Format("/select, {0}{1}{0}", (char)34, filePath);
                    Process.Start(explorerPath, arguments);
                }
            }
            catch (Exception)
            {
                //handle the exception your way!
            }
        }

        private static void ProcessItem(AFCDirectory Dir, IConsole console)
        {
            foreach (var item in Dir.GetItems())
            {
                console.WriteLine(item.Path);
                console.WriteLine($"> {item.CreationTime}");
                console.WriteLine($"> {item.LastModifiedTime}");
                console.WriteLine($"> {item.Name}");
                console.WriteLine($"> {item.Extension}");
            }
            ProcessLine(Dir, Console.ReadLine(), console);
        }

        private static void ProcessItem(AFCFile file, IConsole console)
        {
            var p = Path.Combine(Path.GetTempPath(), file.Name);
            var outp = File.OpenWrite(p);
            var inp = file.OpenRead();
            inp.CopyTo(outp);
            inp.Close();
            outp.Close();
            ShowFileInExplorer(p);
            ProcessItem(file.Parent, console);
        }
        private static void ProcessItem(AFCItem item, IConsole console)
        {
            if (item.GetType() == typeof(AFCFile))
            {
                ProcessItem((AFCFile)item, console);
            }
            else if (item.GetType() == typeof(AFCDirectory))
            {
                ProcessItem((AFCDirectory)item, console);
            }
        }
    }
}
