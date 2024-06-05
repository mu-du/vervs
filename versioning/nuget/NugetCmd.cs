namespace versioning.nuget
{
    /// <summary>
    /// Powershell
    ///  [Environment]::SetEnvironmentVariable("LocalNugetPath", "c:\src\xxx\.nuget\packages" ,"User")
    /// </summary>
    class NugetCmd
    {
        private readonly string repo;
        private readonly string nugetPath;

        private readonly List<string> nuspecs;
        private readonly string[] nuspecFiles;

        public string OutputDirectory { get; set; } = Environment.GetEnvironmentVariable("LocalNugetPath") ?? ".";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repo">Full path of repo directory</param>
        public NugetCmd(string repo)
            : this(repo, new string[] { })
        {
        }

        public NugetCmd(string repo, IEnumerable<string> projects)
        {
            Console.WriteLine($"Repository: {repo}");

            this.repo = repo;
            this.nugetPath = Path.Combine(repo, ".nuget");

            try
            {
                this.nuspecFiles = Directory.GetFiles(nugetPath, "*.nuspec");

                if (projects.Any())
                {
                    this.nuspecFiles = this.nuspecFiles
                    .Where(x => projects.Contains(Path.GetFileNameWithoutExtension(x)))
                    .ToArray();
                }

                this.nuspecs = nuspecFiles
                    .Select(x => Path.GetFileName(x))
                    .OrderBy(x => x)
                    .ToList();

                Console.WriteLine($"Total {nuspecs.Count} .nuspec files");
            }
            catch (DirectoryNotFoundException)
            {
                this.nuspecFiles = new string[] { };
                this.nuspecs = new List<string>();

                Console.WriteLine($"Directory not found: {nugetPath}");
            }
        }

        public void Generate(Version? version)
        {
            CreatePackFile();
            CreatePushFile(version);
            CreateDeleteFile();
            CreateInstallFile(version);
        }


        private int AddHeader(List<string> lines, string cmd)
        {
            lines.Add($"REM Total Nuget Packages: {nuspecs.Count}");
            lines.Add($"REM Usage: ./{cmd} <version>");
            lines.Add($"REM Example: ./{cmd} 1.0.0");
            lines.Add("");

            return lines.Count;
        }

        public void CreatePackFile()
        {
            if (nuspecs.Count == 0)
                return;

            string cmd = "nuget-pack.cmd";

            List<string> lines = new List<string>();
            AddHeader(lines, cmd);

            var packs = nuspecs.Select(x => PackCommand(x, OutputDirectory));
            lines.AddRange(packs);

            Save(cmd, lines);
        }

        public void CreatePushFile(Version? version)
        {
            if (nuspecs.Count == 0)
                return;

            string ver = "%1";
            string cmd = $"nuget-push.cmd";
            if (version != null)
            {
                ver = $"{version.Major}.{version.Minor}.{version.Build}";
                cmd = $"nuget-push-{ver}.cmd";
            }

            List<string> lines = new List<string>();
            AddHeader(lines, cmd);

            var pushes = nuspecs
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .Select(x => $"nuget push {OutputDirectory}\\{x}.{ver}.nupkg -Source https://atmsnuget.azurewebsites.net/api/v2/package");
            lines.AddRange(pushes);

            Save(cmd, lines);
        }

        public void CreateDeleteFile()
        {
            if (nuspecs.Count == 0)
                return;

            string cmd = "nuget-delete.cmd";

            List<string> lines = new List<string>();
            AddHeader(lines, cmd);

            var deletes = nuspecs
                .Select(x => Path.GetFileNameWithoutExtension(x).ToLower())
                .Select(x => $"del /F /S /Q \"%USERPROFILE%\\.nuget\\packages\\{x}\\%1\"");
            lines.AddRange(deletes);

            Save(cmd, lines);
        }


        public void CreateInstallFile(Version? version)
        {
            if (nuspecs.Count == 0)
                return;

            string ver = "%1";
            string cmd = "nuget-install.cmd";
            if (version != null)
            {
                ver = $"{version.Major}.{version.Minor}.{version.Build}";
                cmd = $"nuget-install-{ver}.cmd";
            }


            List<string> lines = new List<string>();
            AddHeader(lines, cmd);

            var deletes = nuspecs
                .Select(x => Path.GetFileNameWithoutExtension(x).ToLower())
                .Select(x => $"install-package {x} -Version {ver}");
            lines.AddRange(deletes);

            Save(cmd, lines);
        }


        private void Save(string cmd, List<string> lines)
        {
            string path = Path.Combine(nugetPath, cmd);
            File.WriteAllLines(path, lines);
            Console.WriteLine($"Generated: {path}");
        }

        private static string PackCommand(string nuspec, string outputDirectory)
        {
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return $"nuget pack {nuspec}";
            else
                return $"nuget pack {nuspec} -OutputDirectory {outputDirectory}";
        }
    }

}
