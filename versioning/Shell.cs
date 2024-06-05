﻿using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using versioning.nuget;

namespace versioning
{
    class Shell
    {
        public Shell()
        {
        }

        private static Version ParseVersion(string ver)
        {
            Version? version;
            if (!Version.TryParse(ver, out version))
            {
                Console.WriteLine($"Wrong version number: {ver}");
                Environment.Exit(-1);
            }
            Console.WriteLine($"Version = {version}");
            return version;
        }

        private static void CheckBuildSrcDirectory(string buildsrc)
        {
            if (!Directory.Exists(buildsrc))
            {
                Console.WriteLine($"Directory not found: {buildsrc}");
                Environment.Exit(-1);
            }

            Console.WriteLine($"Directory = {buildsrc}");
        }

        public void UpdateRepo(string ver, string buildsrc, string envFile)
        {
            Version version = ParseVersion(ver);
            CheckBuildSrcDirectory(buildsrc);


            Versioning update = new Versioning(version);
            update.UpdateVersion(buildsrc);

            if (!string.IsNullOrEmpty(envFile))
            {
                var buildEvent = new BuildEvent(buildsrc, version);
                buildEvent.PrepareBuild(envFile);
            }

            NugetCmd cmd = new NugetCmd(buildsrc);
            cmd.Generate(version: null);
        }



        public void UpdateProject(string ver, string buildsrc, string project)
        {
            Version version = ParseVersion(ver);
            CheckBuildSrcDirectory(buildsrc);

            Versioning update = new Versioning(version);
            var projects = update.UpdateVersion(buildsrc, project);

            NugetCmd cmd = new NugetCmd(buildsrc, projects);
            cmd.Generate(version);

            
        }
    }
}
