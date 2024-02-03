using System;

namespace versioning
{
    internal interface IVersioning
    {
        void UpdateVersion(Version ver);
        void Save();
    }
}