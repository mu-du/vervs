using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using versioning.project;
using versioning.nuget;

namespace versioning
{
    class Versioning
    {
        private Version version;
        public Versioning(Version version)
        {
            this.version = version;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repo">Full path of repo directory</param>
        public void UpdateVersion(string repo)
        {
            //update *.csproj
            var projectRepo = new ProjectRepo(repo);
            projectRepo.UpdateVersion(version);

            //update *.nuspec
            NuspecRepo nuget = new NuspecRepo(repo);
            nuget.UpdateVersion(version);


            //update AssemblyInfo.cs
            var cs = new AssemblyInfoRepo(repo);
            cs.UpdateVersion(version);

            //overwrite Version.cs
            var files = Directory.GetFiles(repo, "Version.cs", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                CreateFWVersion(file, version);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="projectName">Full path of repo directory</param>
        public List<string> UpdateVersion(string repo, string projectName)
        {

            //update *.nuspec
            NuspecRepo nuget = new NuspecRepo(repo);
            List<string> projects = nuget.UpdateVersion(version, projectName);

            var projectRepo = new ProjectRepo(repo);
            foreach (string project in projects)
            {
                projectRepo.UpdateVersion(version, project);
            }

            //update AssemblyInfo.cs
            var cs = new AssemblyInfoRepo(repo, projectName);
            cs.UpdateVersion(version);

            return projects;
        }


        /// <summary>
        /// Update Version.cs
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ver"></param>
        private static void CreateFWVersion(string path, Version ver)
        {
            if (!File.Exists(path))
                return;

            string[] lines = new string[]
            {
                "using System.Reflection;",
                "using System.Runtime.CompilerServices;",
                $"[assembly: AssemblyVersion(\"{ver}\")]",
                $"[assembly: AssemblyFileVersion(\"{ver}\")]"
            };
            File.WriteAllLines(path, lines);
            Console.WriteLine($"Completed {path}");
        }
    }
}
