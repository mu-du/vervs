using System;
using System.IO;

namespace versioning
{
    class Shell
    {
        public Shell(string[] args)
        {
            string ver = "1.0.0.0";
            if (args.Length > 0)
            {
                ver = args[0];
            }

            Version version;
            if (!Version.TryParse(ver, out version))
            {
                Console.WriteLine($"Wrong version number: {ver}");
                Environment.Exit(-1);
            }
            Console.WriteLine($"Version = {version}");


            string buildsrc = Directory.GetCurrentDirectory();
            if (args.Length > 1)
            {
                buildsrc = args[1];
                if (buildsrc.EndsWith("\\"))
                    buildsrc = buildsrc.Substring(0, buildsrc.Length - 1);

                if (!Path.IsPathRooted(buildsrc))
                {
                    string GitHubHome = Environment.GetEnvironmentVariable("GitHubHome");
                    if (GitHubHome != null)
                    {
                        buildsrc = Path.Combine(GitHubHome, buildsrc);
                    }
                }
            }
            
            buildsrc = Path.GetFullPath(buildsrc);
            if (!Directory.Exists(buildsrc))
            {
                Console.WriteLine($"Directory not found: {buildsrc}");
                Environment.Exit(-1);
            }
            Console.WriteLine($"Directory = {buildsrc}");


            Versioning update = new Versioning(version);
            update.UpdateVersion(buildsrc);

            if (args.Length > 2)
            {
                var envFile = args[2];
                var buildEvent = new BuildEvent(buildsrc, version);
                buildEvent.PrepareBuild(envFile);
            }
        }




    }
}
