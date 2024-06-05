using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using static System.Net.WebRequestMethods;

namespace versioning.nuget
{
    class NuspecRepo : IVersioning
    {
        private Dictionary<string, NuspecFile> NuspecFiles { get; } = new Dictionary<string, NuspecFile>();
        private string root;

        public NuspecRepo(string repo)
        {
            this.root = repo;

            string[] files = Directory.GetFiles(root, "*.nuspec", SearchOption.AllDirectories);
            files = files.Where(x => x.IndexOf("\\obj\\Debug\\") == -1).ToArray();

            foreach (string file in files)
            {
                try
                {
                    var nuspec = new NuspecFile(file);
                    if (!NuspecFiles.ContainsKey(nuspec.Id))
                    {
                        NuspecFiles.Add(nuspec.Id, nuspec);
                    }
                    else
                    {
                        var nuspec1 = NuspecFiles[nuspec.Id];
                        if (nuspec1.Version < nuspec.Version)
                        {
                            NuspecFiles[nuspec.Id] = nuspec;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error in processing nuspec file \"{file}\",{ex.Message}");
                }
            }
        }

        /// <summary>
        /// Update version internal repository
        /// </summary>
        /// <param name="ver"></param>
        public void UpdateVersion(Version ver)
        {
            UpdateVersion(ver, new NuspecRepo[] { });
        }

        public void UpdateVersion(Version ver, string project)
        {
            List<NuspecFile> files = new List<NuspecFile>();
            foreach (NuspecFile nuspec in NuspecFiles.Values)
            {
                if (nuspec.Id == project)
                {
                    nuspec.Version = ver;
                    files.Add(nuspec);
                }
                else
                {
                    var dependencies = nuspec.GetDependecies();
                    if (dependencies.Contains(project))
                    {
                        nuspec.Version = ver;
                        files.Add(nuspec);
                    }
                }
            }

            UpdateVersion(ver, files);
        }

        /// <summary>
        /// Update dependency version from other repositories
        /// </summary>
        /// <param name="ver"></param>
        /// <param name="repos"></param>
        public void UpdateVersion(Version ver, IEnumerable<NuspecRepo> repos)
        {
            List<NuspecFile> files = new List<NuspecFile>();
            foreach (var nuspec in NuspecFiles.Values)
            {
                //use the new version in the same repo
                nuspec.Version = ver;
                files.Add(nuspec);
            }

            foreach (var repo in repos)
            {
                files.AddRange(repo.NuspecFiles.Values);
            }

            UpdateVersion(ver, NuspecFiles.Values);
        }

        private void UpdateVersion(Version ver, IEnumerable<NuspecFile> nuspecFiles)
        {
            foreach (var nuspec in nuspecFiles)
            {
                nuspec.UpdateVersion(ver);
                nuspec.UpdateDependecies(nuspecFiles);
                nuspec.CreateReleaseNotes(ver);
                nuspec.Save();
            }
        }

        public void Save()
        {

        }

        public override string ToString()
        {
            return root;
        }
    }
}
