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


        const string BUILDSRC = "BUILDSRC";
        const string BUILDVER = "BUILDVER";
        const string BUILDNEXT = "BUILDNEXT";

        public void PrepareBuild(string fileName)
        {
            string fullPath = GetFullPath(fileName);

            List<string> lines = new List<string>();
            if (File.Exists(fullPath))
            {
                lines.AddRange(File.ReadAllLines(fullPath));
            }

            string ext = Path.GetExtension(fileName);
            if (ext == ".ps1")
            {
                Replace(lines, BUILDSRC, $"$env:{BUILDSRC}=\"{buildsrc}\"");
                Replace(lines, BUILDVER, $"$env:{BUILDVER}=\"{VersionName}\"");
                Replace(lines, BUILDNEXT, $"$env:{BUILDNEXT}=\"{NextBuild}\"");
            }
            else
            {
                Replace(lines, BUILDSRC, $"set {BUILDSRC}={buildsrc}");
                Replace(lines, BUILDVER, $"set {BUILDVER}={VersionName}");
                Replace(lines, BUILDNEXT, $"set {BUILDNEXT}={NextBuild}");
            }

            Environment.SetEnvironmentVariable(BUILDSRC, buildsrc);
            Environment.SetEnvironmentVariable(BUILDVER, VersionName);
            Environment.SetEnvironmentVariable(BUILDNEXT, NextBuild);


            File.WriteAllLines(fullPath, lines);
            Console.WriteLine($"Created {fullPath}");
        }

        private string GetFullPath(string fileName)
        {
            string path;
            if (!Path.IsPathRooted(fileName))
                path = Path.Combine(buildsrc, fileName);
            else
                path = Path.GetFullPath(fileName);

            string? directory = Path.GetDirectoryName(path);
            if (directory != null && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return path;
        }

        private static void Replace(List<string> lines, string key, string value)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].IndexOf(key) >= 0)
                {
                    lines[i] = value;
                    return;
                }
            }

            lines.Add(value);
        }
    }
}
