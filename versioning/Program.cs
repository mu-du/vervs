using System;
using System.IO;

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

            new BuildEvent(buildsrc, version).PrepareBuild();
        }


        private static void Help()
        {
            Console.WriteLine($"Versioning for Visual Studio, v{System.Reflection.Assembly.GetEntryAssembly().GetName().Version}");
            Console.WriteLine("     Copyright Mudu (c) 2021. All rights reserved");
            Console.WriteLine("Usage:");
            Console.WriteLine("  vervs <version-number> <build-source-directory>");
            Console.WriteLine("  vervs <version-number> <repo-name>");
            Console.WriteLine("  Note: Setup environment variable [GitHubHome] if repo-name used");
            Console.WriteLine("Examples:");
            Console.WriteLine("  vervs 1.0.9.0 c:\\devel\\GitHub\\sqlcode");
            Console.WriteLine("  vervs 1.0.9.0 sqlcode");
            Console.WriteLine("  vervs 1.0.9.0");
        }

    }
}
