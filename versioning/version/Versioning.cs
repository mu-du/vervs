﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using Versioning.Project;
using Versioning.NuGet;

namespace Versioning.version
{
    class Versioning
    {
        private readonly Version version;

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

            //overwrite app.rc
            files = Directory.GetFiles(repo, "app.rc", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                CreateRCVersion(file, version);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="projectName">Full path of repo directory</param>
        public List<string> UpdateProjectVersion(string repo, string projectName)
        {

            //update *.nuspec
            NuspecRepo nuget = new NuspecRepo(repo);
            List<string> projects = nuget.UpdateProjectVersion(version, projectName);

            var projectRepo = new ProjectRepo(repo);
            foreach (string project in projects)
            {
                projectRepo.UpdateProjectVersion(version, project);
            }

            //update AssemblyInfo.cs
            var cs = new AssemblyInfoRepo(repo, projectName);
            cs.UpdateVersion(version);

            return projects;
        }

        public void UpdatePackageVersion(string repo, string packageId)
        {
            NuspecRepo nuget = new NuspecRepo(repo);
            nuget.UpdatePackageVersion(packageId, version);
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

        private static void CreateRCVersion(string path, Version ver)
        {
            if (!File.Exists(path))
                return;

            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Replace(lines[i], "FILEVERSION", ver.ToString4c());
                lines[i] = Replace(lines[i], "PRODUCTVERSION", ver.ToString4c());
                lines[i] = Replace(lines[i], $"{Quote("FileVersion")},", Quote(ver.ToString4()));
                lines[i] = Replace(lines[i], $"{Quote("ProductVersion")},", Quote(ver.ToString4()));
            }


            File.WriteAllLines(path, lines);
            Console.WriteLine($"Completed {path}");
        }

        private static string Quote(string text)
        {
            return $"\"{text}\"";
        }

        private static string Replace(string line, string key, string ver)
        {
            key += " ";
            int index = line.IndexOf(key);

            if (index != -1)
            {
                return line.Substring(0, index + key.Length) + ver;
            }

            return line;
        }
    }
}
