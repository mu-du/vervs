using System;
using System.IO;

namespace versioning
{
    class ProjectRepo
    {
        private string[] files;
        public ProjectRepo(string repo)
        {
            this.files = Directory.GetFiles(repo, "*.csproj", SearchOption.AllDirectories);
        }

        public void UpdateVersion(Version ver)
        {
            foreach (string file in files)
            {
                var csproj = new ProjectFile(file);
                csproj.UpdateVersion(ver);
                csproj.Save();
            }
        }
    }
}
