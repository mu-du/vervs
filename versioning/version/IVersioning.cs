using System;

namespace versioning.version
{
    internal interface IVersioning
    {
        void UpdateVersion(Version ver);
        void Save();
    }
}