﻿using System;
using System.IO;

namespace Versioning
{
    class AssemblyInfoRepo
    {
        private readonly string repo;
        private readonly string[] files;

        public AssemblyInfoRepo(string repo)
        {
            this.repo = repo;

            this.files = Directory.GetFiles(repo, "AssemblyInfo.cs", SearchOption.AllDirectories);
        }

        public AssemblyInfoRepo(string repo, string project)
        {
            this.repo = repo;

            this.files = Directory.GetFiles(Path.Combine(repo, project), "AssemblyInfo.cs", SearchOption.AllDirectories);
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
