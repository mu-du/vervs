using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.IO;
using System.Xml.Linq;
using versioning.version;

namespace versioning.project
{

    class ProjectFile : IVersioning
    {
        readonly XNamespace xmlns = "urn:oasis:names:tc:xliff:document:1.2";

        private readonly string path;
        private readonly XElement? project;
        private readonly XElement? propertyGroup;

        private readonly string[] sdks = new string[]
        {
            "Microsoft.NET.Sdk",
            "Microsoft.NET.Sdk.Web",
            "Microsoft.NET.Sdk.Worker",
        };

        public ProjectFile(string path)
        {
            this.path = path;
            this.project = XElement.Load(path);

            string? sdk = (string?)project.Attribute("Sdk");
            if (!sdks.Contains(sdk))
            {
                Console.WriteLine($"not .net core project: \"{path}\"");
                return;
            }

            this.propertyGroup = project.Element("PropertyGroup");
        }


        private void AddOrUpdateElement(XElement? parent, string name, string value)
        {
            XElement? element = parent?.Element(name);
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

        /// <summary>
        /// Update project files
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ver"></param>
        public void UpdateVersion(Version ver)
        {
            //none .net core project
            if (propertyGroup == null)
                return;

            SetValue("Version", ver.ToString3(), addIfNotExist: true);
            SetValue("AssemblyVersion", ver.ToString());
            SetValue("FileVersion", ver.ToString());

            UpateTemporarily(ver);
        }

        private void UpateTemporarily(Version ver)
        {
            string? packageId = GetValue("PackageId");
            string? name = packageId?.Replace("tw.", "").Replace(".", " ");

            UpdateAuthors("Fuhua Jiang");
            UpdateDescription($"Atms Core {packageId} Library");
            UpdateCompany("Cubic-ITS");
            UpdateCopyright("Copyright 2020-2024");
            UpdatePropertyGroup("PackageProjectUrl", "https://bitbucket.org/cubic-its/atms.core");
            UpdatePropertyGroup("Title", $"Trafficware {name}");
            UpdatePropertyGroup("PackageReleaseNotes", ReleaseNotes(ver));
        }

        private string ReleaseNotes(Version ver)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine($"{DateTime.Today:MM/dd/yyyy} v{ver.ToString3()}");
            sb.AppendLine("Improvements/Enhancements:");
            sb.AppendLine("1.");
            sb.AppendLine("2.");
            sb.AppendLine("Bug Fixes:");
            sb.AppendLine("1.");
            sb.AppendLine("2.");

            return sb.ToString();
        }

        private string? GetValue(string key)
        {
            XElement? xElement = propertyGroup?.Element(key);
            if (xElement != null)
            {
                return xElement.Value;
            }

            return null;
        }

        private bool SetValue(string key, string newValue, bool addIfNotExist = false)
        {
            XElement? xElement = propertyGroup?.Element(key);
            if (xElement != null)
            {
                string oldValue = xElement.Value;
                if (oldValue != newValue)
                {
                    xElement.Value = newValue;
                    Console.WriteLine($"{key} = {oldValue} -> {newValue}");
                    return true;
                }
            }
            else if (addIfNotExist)
            {
                xElement = new XElement(key, newValue);
                propertyGroup?.Add(xElement);
                Console.WriteLine($"{key} = {newValue}");
            }

            return false;
        }

        public void UpdateAuthors(string value)
        {
            UpdatePropertyGroup("Authors", value);
        }

        public void UpdateDescription(string value)
        {
            UpdatePropertyGroup("Description", value);
        }

        public void UpdateCopyright(string value)
        {
            UpdatePropertyGroup("Copyright", value);
        }

        public void UpdateCompany(string value)
        {
            UpdatePropertyGroup("Company", value);
        }

        public void UpdatePropertyGroup(string tag, string value)
        {
            AddOrUpdateElement(propertyGroup, tag, value);
        }


        public void Save()
        {
            project?.Save(path, SaveOptions.OmitDuplicateNamespaces);
            Console.WriteLine($"Completed {Path.GetFileNameWithoutExtension(path)}");
        }

    }
}
