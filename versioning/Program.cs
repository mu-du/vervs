using System;
using System.IO;
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

            string buildsrc = Directory.GetCurrentDirectory();
            string ver = "1.0.0.0";

            if (args.Length > 0)
            {
                ver = args[0];

                if (args.Length > 1)
                {
                    buildsrc = args[1];
                    if (buildsrc.EndsWith("\\"))
                        buildsrc = buildsrc.Substring(0, buildsrc.Length - 1);

                    if (!Path.IsPathRooted(buildsrc))
                    {
                        string GitHubHome = Environment.GetEnvironmentVariable("GitHubHome");
                        buildsrc = Path.Combine(GitHubHome, buildsrc);
                    }
                }
            }

            if (!Directory.Exists(buildsrc))
            {
                Console.WriteLine($"Directory not found: {buildsrc}");
                Environment.Exit(-1);
            }
            Console.WriteLine($"Directory = {buildsrc}");

            Version version;
            if (!Version.TryParse(ver, out version))
            {
                Console.WriteLine($"Wrong version number: {ver}");
                Environment.Exit(-1);
            }
            Console.WriteLine($"Version = {version}");

            Versioning update = new Versioning(version);
            update.UpdateVersion(buildsrc);

            if (args.Length > 2)
            {
                var envFile = args[2];
                var buildEvent = new BuildEvent(buildsrc, version);
                buildEvent.PrepareBuild(envFile);
            }
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
