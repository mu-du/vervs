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

        public void UpdateVersion(string rootPath)
        {
            //update *.csproj
            var projects = new ProjectRepo(rootPath);
            projects.UpdateVersion(version);

            //update *.nuspec
            NuspecRepo nuget = new NuspecRepo(rootPath);
            nuget.UpdateVersion(version);


            //update AssemblyInfo.cs
            var cs = new AssemblyInfoRepo(rootPath);
            cs.UpdateVersion(version);

            //overwrite Version.cs
            var files = Directory.GetFiles(rootPath, "Version.cs", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                CreateFWVersion(file, version);
            }

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
