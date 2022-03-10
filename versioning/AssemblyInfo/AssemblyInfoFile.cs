using System;
using System.IO;

namespace versioning
{
    class AssemblyInfoFile
    {
        private string path;
        private string[] lines;
        bool dirty = false;

        public AssemblyInfoFile(string path)
        {
            this.path = path;
            if (!File.Exists(path))
                return;

            this.lines = File.ReadAllLines(path);
        }



        public void UpdateVersion(Version ver)
        {
            if (lines == null)
                return;

            dirty = false;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                int p1 = line.IndexOf("//");
                int p2 = line.IndexOf("AssemblyVersion");

                if (p2 >= 0 && (p1 == -1 || p1 > p2))
                {
                    lines[i] = $"[assembly: AssemblyVersion(\"{ver}\")]";
                    dirty = true;
                }

                p2 = line.IndexOf("AssemblyFileVersion");
                if (p2 >= 0 && (p1 == -1 || p1 > p2))
                {
                    lines[i] = $"[assembly: AssemblyFileVersion(\"{ver}\")]";
                    dirty = true;
                }
            }
        }

        public void Save()
        {
            if (lines == null)
                return;

            if (dirty)
            {
                File.WriteAllLines(path, lines);
            }

            Console.WriteLine($"Completed {path}");
        }

        public override string ToString() => path;
    }
}
