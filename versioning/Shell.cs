using System;
using System.IO;
using System.Runtime.ConstrainedExecution;

namespace versioning
{
    class Shell
    {
        public Shell()
        {
        }

        private static Version ParseVersion(string ver)
        {
            Version version;
            if (!Version.TryParse(ver, out version))
            {
                Console.WriteLine($"Wrong version number: {ver}");
                Environment.Exit(-1);
            }
            Console.WriteLine($"Version = {version}");
            return version;
        }


        public void UpdateRepo(string ver, string buildsrc, string envFile)
        {
            Version version = ParseVersion(ver);

            if (!Directory.Exists(buildsrc))
            {
                Console.WriteLine($"Directory not found: {buildsrc}");
                Environment.Exit(-1);
            }

            Console.WriteLine($"Directory = {buildsrc}");

            Versioning update = new Versioning(version);
            update.UpdateVersion(buildsrc);

            if (!string.IsNullOrEmpty(envFile))
            {
                var buildEvent = new BuildEvent(buildsrc, version);
                buildEvent.PrepareBuild(envFile);
            }
        }

        public void UpdateProject(string ver, string repo, string project)
        {
            Version version = ParseVersion(ver);
            Versioning update = new Versioning(version);
            update.UpdateVersion(repo);

            
        }
    }
}
