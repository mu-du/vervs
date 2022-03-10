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

        public string VersionName => $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";

        public void PrepareBuild()
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine($"set BUILDSRC={buildsrc}");
            code.AppendLine($"set BUILDVER={VersionName}");

            Environment.SetEnvironmentVariable("BUILDSRC", buildsrc);
            Environment.SetEnvironmentVariable("BUILDVER", VersionName);

            string path = Path.Combine(buildsrc, "vervs.cmd");
            if (!Directory.Exists(buildsrc))
                Directory.CreateDirectory(buildsrc);

            File.WriteAllText(path, code.ToString());
            Console.WriteLine($"created {path}");
        }
    }
}
