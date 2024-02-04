using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace versioning.project
{
    internal class VdProjectFile : IVersioning
    {
        private string path;
        private string[] project;

        public VdProjectFile(string path)
        {
            this.path = path;
            this.project = File.ReadAllLines(path);
        }

        public void UpdateVersion(Version ver)
        {
            const string _ProductCode = "\"ProductCode\"";
            const string _PackageCode = "\"PackageCode\"";
            const string _ProductVersion = "\"ProductVersion\"";

            var L1 = project.Where(x => x.IndexOf(_ProductCode) > 0).ToArray();
            var L2 = project.Where(x => x.IndexOf(_PackageCode) > 0).ToArray();
            var L3 = project.Where(x => x.IndexOf(_ProductVersion) > 0).ToArray();

            if (SetValue("ProductVersion", "8:", "", $"{ver.Major}.{ver.Minor}.{ver.Build}"))
            {
                SetValue("ProductCode", "8:{", "}", $"{Guid.NewGuid().ToString().ToUpper()}");
                SetValue("PackageCode", "8:{", "}", $"{Guid.NewGuid().ToString().ToUpper()}");
            }
        }

        private bool SetValue(string key, string prefix, string suffix, string value)
        {
            key = $"\"{key}\"";

            string _key = null;
            string oldValue = null;

            int i = 0;
            while (i < project.Length)
            {
                var line = project[i];
                if (line.IndexOf(key) > 0)
                {
                    int index = line.IndexOf("=");
                    string left = line.Substring(0, index);
                    string right = line.Substring(index + 1);
                    _key = GetItem(left);
                    oldValue = GetItem(right);

                    if (oldValue.StartsWith(prefix) && oldValue.EndsWith(suffix))
                    {
                        break;
                    }
                }

                i++;
            }

            if (oldValue != null)
            {
                string newValue = $"{prefix}{value}{suffix}";
                if (oldValue != newValue)
                {
                    var newLine = project[i].Replace(oldValue, newValue);
                    project[i] = newLine;
                    Console.WriteLine($"{_key} = {oldValue} -> {newValue}");
                    return true;
                }
            }
            else
            {
                Console.Error.WriteLine($"Cannot find key: {key} in file: {path}");
            }

            return false;
        }

        private string GetItem(string item)
        {
            item = item.Trim();
            if (item.StartsWith('\"') && item.EndsWith('"'))
            {
                item = item[1..^1];
            }

            return item;
        }

        public void Save()
        {
            File.WriteAllLines(path, project);
            Console.WriteLine($"Completed {Path.GetFileNameWithoutExtension(path)}");
        }
    }
}
