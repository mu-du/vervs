using System;
using System.IO;

namespace versioning
{
    class AssemblyInfoRepo
    {
        private string repo;
        private string[] files;

        public AssemblyInfoRepo(string repo)
        {
            this.repo = repo;

            this.files = Directory.GetFiles(repo, "AssemblyInfo.cs", SearchOption.AllDirectories);
        }

        public void UpdateVersion(Version version)
        { 
            foreach (string file in files)
            {
                var cs = new AssemblyInfoFile(file);
                cs.UpdateVersion(version);
                cs.Save();
            }
        }
    }
}
