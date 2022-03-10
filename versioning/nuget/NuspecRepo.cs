using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace versioning
{
    class NuspecRepo
    {
        private Dictionary<string, NuspecFile> NuspecFiles { get; } = new Dictionary<string, NuspecFile>();
        private string root;

        public NuspecRepo(string repo)
        {
            this.root = repo;

            string[] files = Directory.GetFiles(root, "*.nuspec", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                try
                {
                    var nuspec = new NuspecFile(file);
                    NuspecFiles.Add(nuspec.Id, nuspec);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
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

        public override string ToString()
        {
            return root;
        }
    }
}
