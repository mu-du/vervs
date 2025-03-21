﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Versioning.version;

namespace Versioning.NuGet
{
    class NuspecFile
    {
        private readonly string path;
        private readonly XNamespace xmlns = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd";
        private readonly XElement? package;
        private readonly XElement? metadata;

        public string Id { get; }
        public Version? Version { get; set; }

        public NuspecFile(string path)
        {
            this.path = path;

            this.package = XElement.Load(path);
            this.xmlns = package.GetDefaultNamespace();

            this.metadata = package.Element(xmlns + "metadata");
            XElement? id = metadata?.Element(xmlns + "id");

            if (id == null)
            {
                throw new Exception($"Cannot find <id/> in \"{path}\"");
            }

            this.Id = (string)id;

            XElement? version = metadata?.Element(xmlns + "version");
            if (version != null)
            {
                Version = new Version(version.Value);
            }
        }

        private void AddOrUpdateElement(XElement? parent, string name, string value)
        {
            XElement? element = parent?.Element(xmlns + name);
            if (element != null)
            {
                element.Value = value;
            }
            else
            {
                element = new XElement(name, value);
                parent?.Add(element);
            }
        }

        public void UpdateVersion(Version ver)
        {
            this.Version = ver;
            string _ver = ver.ToString3();
            UpdateMetadataChild("version", _ver);
        }

        public void UpdateAuthors(string value)
        {
            UpdateMetadataChild("authors", value);
        }

        public void UpdateDescription(string value)
        {
            UpdateMetadataChild("description", value);
        }

        public void UpdateCopyright(string value)
        {
            UpdateMetadataChild("copyright", value);
        }

        public void UpdateCompany(string value)
        {
            UpdateMetadataChild("company", value);
        }

        public void UpdateMetadataChild(string tag, string value)
        {
            AddOrUpdateElement(metadata, tag, value);
        }

        public void Save()
        {
            package?.Save(path, SaveOptions.OmitDuplicateNamespaces);
            Console.WriteLine($"Completed {path}");
        }

        public void UpdateDependecies(IEnumerable<NuspecFile> nuspecFiles)
        {
            XElement? dependencies = metadata?.Element(xmlns + "dependencies");
            if (dependencies != null)
            {
                var groups = dependencies.Elements(xmlns + "group");
                foreach (var group in groups)
                {
                    var dependencyElements = group.Elements(xmlns + "dependency");
                    foreach (var dependency in dependencyElements)
                    {
                        var _id = (string?)dependency.Attribute("id");
                        if (_id != null)
                        {
                            var first = nuspecFiles.FirstOrDefault(x => x.Id == _id);
                            if (first != null)
                                dependency.SetAttributeValue("version", (first.Version?.ToString3()));
                        }
                    }
                }
            }
        }

        public List<string> GetDependecies()
        {
            List<string> list = new List<string>();
            XElement? dependencies = metadata?.Element(xmlns + "dependencies");
            if (dependencies != null)
            {
                var groups = dependencies.Elements(xmlns + "group");
                foreach (var group in groups)
                {
                    var dependencyElements = group.Elements(xmlns + "dependency");
                    foreach (var dependency in dependencyElements)
                    {
                        var _id = (string?)dependency.Attribute("id");
                        if (_id != null)
                        {
                            list.Add(_id);
                            string? ver = (string?)dependency.Attribute("version");
                        }
                    }
                }
            }

            return list;
        }

        public void UpdatePackageVersion(string packageId, Version version)
        {
            XElement? dependencies = metadata?.Element(xmlns + "dependencies");
            if (dependencies != null)
            {
                bool changed = false;
                var groups = dependencies.Elements(xmlns + "group");
                foreach (var group in groups)
                {
                    var dependencyElements = group.Elements(xmlns + "dependency");
                    foreach (var dependency in dependencyElements)
                    {
                        var _id = (string?)dependency.Attribute("id");
                        if (_id != null && _id == packageId)
                        {
                            var oldVersion = (string?)dependency.Attribute("version");
                            dependency.SetAttributeValue("version", version?.ToString3());
                            Console.WriteLine($"Package {packageId} {oldVersion} -> {version}");
                            changed = true;
                        }
                    }
                }

                if (changed)
                    Save();
            }
        }

        public void CreateReleaseNotes(Version verison)
        {
            string ver = verison.ToString3();

            XElement? releaseNotes = metadata?.Element(xmlns + "releaseNotes");
            if (releaseNotes == null)
            {
                releaseNotes = new XElement("releaseNotes", string.Empty);
                metadata?.Add(releaseNotes);
            }

            bool found = false;
            using (var reader = new StringReader(releaseNotes.Value))
            {
                while (true)
                {
                    string? line = reader.ReadLine();
                    if (line == null)
                        break;

                    if (line.EndsWith(ver))
                    {
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                string space = "\t\t";
                DateTime today = DateTime.Today;
                StringBuilder notes = new StringBuilder();
                notes.AppendLine();
                notes.AppendLine($"{space}{today.Month}/{today.Day}/{today.Year} v{ver}");
                notes.AppendLine($"{space}Improvements/Enhancements:");
                notes.AppendLine($"{space}1.");
                notes.AppendLine($"{space}2.");
                notes.AppendLine($"{space}Bug Fixes:");
                notes.AppendLine($"{space}1.");
                notes.AppendLine($"{space}2.");
                Format(notes, space, releaseNotes.Value);
                releaseNotes.Value = notes.ToString();
            }
        }

        private void Format(StringBuilder notes, string space, string text)
        {
            string[] lines = text.Split(Environment.NewLine);
            foreach (string line in lines)
            {
                notes.AppendLine($"{space}{line.Trim()}");
            }
        }



        public override string ToString()
        {
            return $"Id={Id}, Ver={Version}";
        }
    }
}
