using System;
using System.IO;
using versioning.nuget;

namespace versioning
{
    class Shell
    {
        public const string UNDEFINED = "N/A";

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
            repo = Path.GetFullPath(repo);

            if (!Directory.Exists(repo))
            {
                History.Error($"Directory not found: {repo}");
                Environment.Exit(-2);
            }

            string[] directories = Directory.GetDirectories(repo)
                .Select(x => Path.GetFileName(x))
                .ToArray();

            if (!IsRepoDirectory(repo))
            {
                History.Error($"Invalid git repository: {repo}");
                Environment.Exit(-3);
            }

            Console.WriteLine($"Reposity Directory = {repo}");
        }


        public static bool IsRepoDirectory(string repo)
        {
            string[] directories = Directory.GetDirectories(repo)
                .Select(x => Path.GetFileName(x))
                .ToArray();

            return directories.Contains(".git");
        }


        public void UpdateRepo(string ver, string repo, string envFile, bool nugetCmd)
        {
            CheckRepoDirectory(repo);
            Version version = ParseVersion(ver);

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
            if (repo == UNDEFINED)
            {
                History.Error("Repo is expected.");
                return;
            }

            if (project == UNDEFINED)
            {
                History.Error("Project name is expected.");
                return;
            }

            CheckRepoDirectory(repo);
            Version version = ParseVersion(ver);

            Console.WriteLine($"Project = {project}");

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

            CheckRepoDirectory(repo);
            Version version = ParseVersion(ver);

            Console.WriteLine($"PackageId = {packageId}");

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
