using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace versioning
{
    class BuildEvent
    {
        private readonly string buildsrc;
        private readonly Version version;

        public BuildEvent(string buildsrc, Version version)
        {
            this.buildsrc = buildsrc;
            this.version = version;
        }

        public string VersionName => version.ToString4();
        public string NextBuild => new Version(version.Major, version.Minor, version.Build + 1, version.Revision).ToString4();

        public void PrepareBuild(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            StringBuilder code = new StringBuilder();
            if (ext == ".ps1")
            {
                code.AppendLine($"$env:BUILDSRC=\"{buildsrc}\"");
                code.AppendLine($"$env:BUILDVER={VersionName}");
                code.AppendLine($"$env:BUILDNEXT={NextBuild}");
            }
            else
            {
                code.AppendLine($"set BUILDSRC={buildsrc}");
                code.AppendLine($"set BUILDVER={VersionName}");
                code.AppendLine($"set BUILDNEXT={NextBuild}");
            }

            Environment.SetEnvironmentVariable("BUILDSRC", buildsrc);
            Environment.SetEnvironmentVariable("BUILDVER", VersionName);
            Environment.SetEnvironmentVariable("BUILDNEXT", NextBuild);

            string path;
            if (!Path.IsPathRooted(fileName))
                path = Path.Combine(buildsrc, fileName);
            else
                path = Path.GetFullPath(fileName);

            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(path, code.ToString());
            Console.WriteLine($"created {path}");
        }
    }
}
