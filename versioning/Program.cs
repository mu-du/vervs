using System;
using System.Linq;

namespace versioning
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "C# Code Versioning";
            if (args.Length == 0)
            {
                Help();
                return;
            }

            new Shell(args);
        }

        private static void Help()
        {
            Console.WriteLine($"Versioning for Visual Studio, v{System.Reflection.Assembly.GetEntryAssembly().GetName().Version}");
            Console.WriteLine("     Copyright Mudu (c) 2024. All rights reserved");
            Console.WriteLine("Usage:");
            Console.WriteLine("  vervs <version-number> <build-source-directory> [<env-file-name>]");
            Console.WriteLine("  vervs <version-number> <repo-name> [<env-file-name>]");
            Console.WriteLine("Notes:");
            Console.WriteLine("  Setup environment variable [GitHubHome] if repo-name used");
            Console.WriteLine("  <env-file-name> .cmd: command script; .ps1: PowerShell script");
            Console.WriteLine("Examples:");
            Console.WriteLine("  vervs 1.0.9.0 c:\\devel\\mudu\\vervs vervs.cmd");
            Console.WriteLine("  vervs 1.0.9.0 c:\\devel\\mudu\\vervs");
            Console.WriteLine("  vervs 1.0.9.0 vervs");
            Console.WriteLine("  vervs 1.0.9.0");
            Console.WriteLine();
        }
    }
}
