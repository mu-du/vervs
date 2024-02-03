using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace versioning.project
{

    class ProjectFile : IVersioning
    {
        XNamespace xmlns = "urn:oasis:names:tc:xliff:document:1.2";

        private string path;
        private XElement project;
        private XElement propertyGroup;

        private string[] sdks = new string[]
        {
            "Microsoft.NET.Sdk",
            "Microsoft.NET.Sdk.Web",
            "Microsoft.NET.Sdk.Worker",
        };

        public ProjectFile(string path)
        {
            this.path = path;
            this.project = XElement.Load(path);

            string sdk = (string)project.Attribute("Sdk");
            if (!sdks.Contains(sdk))
            {
                Console.WriteLine($"not .net core project: \"{path}\"");
                return;
            }

            this.propertyGroup = project.Element("PropertyGroup");
        }


        private void AddOrUpdateElement(XElement parent, string name, string value)
        {
            XElement element = parent.Element(xmlns + name);
            if (element != null)
            {
                element.Value = value;
            }
            else
            {
                element = new XElement(name, value);
                parent.Add(element);
            }
        }


        private static string ToString(Version version)
        {
            return $"{version.Major}.{version.Minor}.{version.Build}";
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

            XElement version = propertyGroup.Element("Version");
            if (version != null)
            {
                version.Value = $"{ver.Major}.{ver.Minor}.{ver.Build}";
            }
            else
            {
                version = new XElement("Version", $"{ver.Major}.{ver.Minor}.{ver.Build}");
                propertyGroup.Add(version);
            }

            XElement assemblyVersion = propertyGroup.Element("AssemblyVersion");
            if (assemblyVersion != null)
            {
                assemblyVersion.Value = ver.ToString();
            }

            XElement fileVersion = propertyGroup.Element("FileVersion");
            if (fileVersion != null)
            {
                fileVersion.Value = ver.ToString();
            }
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
            project.Save(path, SaveOptions.OmitDuplicateNamespaces);
            Console.WriteLine($"Completed {path}");
        }

    }
}
