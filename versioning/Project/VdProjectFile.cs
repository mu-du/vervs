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

            SetValue("ProductCode", "8:{", "}", $"{Guid.NewGuid().ToString().ToUpper()}");
            SetValue("PackageCode", "8:{", "}", $"{Guid.NewGuid().ToString().ToUpper()}");
            SetValue("ProductVersion", "8:", "", $"{ver.Major}.{ver.Minor}.{ver.Build}");
        }

        private void SetValue(string key, string prefix, string suffix, string value)
        {
            key = $"\"{key}\"";

            string _key = null;
            string _value = null;

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
                    _value = GetItem(right);

                    if (_value.StartsWith(prefix) && _value.EndsWith(suffix))
                    {
                        break;
                    }
                }

                i++;
            }

            if (_value != null)
            {
                var newLine = project[i].Replace(_value, $"{prefix}{value}{suffix}");
                project[i] = newLine; 
            }
            else
            {
                Console.Error.WriteLine($"Cannot find key: {key} in file: {path}");
            }
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
        }
    }
}
