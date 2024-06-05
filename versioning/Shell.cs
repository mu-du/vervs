using System;
using System.IO;
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
                History.Error($"Invalid version number: {ver}");
                Environment.Exit(-1);
            }

            Console.WriteLine($"Version = {version}");
            return version;
        }

        /// <summary>
        /// It is valid repo directory?
        /// </summary>
        /// <param name="repo"></param>
        private static void CheckRepoDirectory(string repo)
        {
            if (!Directory.Exists(repo))
            {
                History.Error($"Directory not found: {repo}");
                Environment.Exit(-2);
            }

            string[] directories = Directory.GetDirectories(repo)
                .Select(x => Path.GetFileName(x))
                .ToArray();

            if (!directories.Contains(".git"))
            {
                History.Error($"Invalid git repository: {repo}");
                Environment.Exit(-3);
            }

            Console.WriteLine($"Directory = {repo}");
        }

        public void UpdateRepo(string ver, string repo, string envFile, bool nugetCmd)
        {
            Version version = ParseVersion(ver);
            CheckRepoDirectory(repo);

            try
            {
                Versioning update = new Versioning(version);
                update.UpdateVersion(repo);

                if (!string.IsNullOrEmpty(envFile))
                {
                    var buildEvent = new BuildEvent(repo, version);
                    buildEvent.PrepareBuild(envFile);
                }

                if (nugetCmd)
                {
                    NugetCmd cmd = new NugetCmd(repo, version);
                    cmd.Generate();
                }
            }
            catch (Exception ex)
            {
                History.Error(ex.Message);
            }
        }



        public void UpdateProject(string ver, string repo, string project, bool nugetCmd)
        {
            Console.WriteLine($"Project = {project}");

            Version version = ParseVersion(ver);
            CheckRepoDirectory(repo);

            try
            {
                Versioning update = new Versioning(version);
                var projects = update.UpdateProjectVersion(repo, project);

                if (nugetCmd)
                {
                    NugetCmd cmd = new NugetCmd(repo, version, projects);
                    cmd.Generate();
                }
            }
            catch (Exception ex)
            {
                History.Error(ex.Message);
            }
        }

        public void UpdatePackage(string packageId, string ver, string repo)
        {
            Console.WriteLine($"PackageId = {packageId}");

            Version version = ParseVersion(ver);
            CheckRepoDirectory(repo);

            try
            {
                Versioning update = new Versioning(version);
                update.UpdatePackageVersion(repo, packageId);
            }
            catch (Exception ex)
            {
                History.Error(ex.Message);
            }
        }
    }
}
