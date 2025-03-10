using System;

namespace Versioning.version
{
    internal interface IVersioning
    {
        void UpdateVersion(Version ver);
        void Save();
    }
}