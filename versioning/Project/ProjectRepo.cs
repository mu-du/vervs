using System;
using System.IO;

namespace versioning.project
{
    class ProjectRepo : IVersioning
    {
        private readonly string repo;
        private string[] csProjectFiles;
        private readonly string[] vdProjectFiles;

        public ProjectRepo(string repo)
        {
            this.repo = repo;

            this.csProjectFiles = Directory.GetFiles(repo, "*.csproj", SearchOption.AllDirectories);
            this.vdProjectFiles = Directory.GetFiles(repo, "*.vdproj", SearchOption.AllDirectories);
        }

        public void UpdateVersion(Version ver)
        {
            foreach (string file in csProjectFiles)
            {
                var csproj = new ProjectFile(file);
                Console.WriteLine($"Process: {file}");
                csproj.UpdateVersion(ver);
                csproj.Save();
            }

            foreach (string file in vdProjectFiles)
            {
                var vdproj = new VdProjectFile(file);
                Console.WriteLine($"Process: {file}");
                vdproj.UpdateVersion(ver);
                vdproj.Save();
            }
        }

        public void UpdateProjectVersion(Version ver, string projectName)
        {
            this.csProjectFiles = Directory.GetFiles(Path.Combine(repo, projectName), "*.csproj", SearchOption.AllDirectories);

            foreach (string file in csProjectFiles)
            {
                var csproj = new ProjectFile(file);
                Console.WriteLine($"Process: {file}");
                csproj.UpdateVersion(ver);
                csproj.Save();
            }

        }

        public void Save()
        {

        }
    }
}
