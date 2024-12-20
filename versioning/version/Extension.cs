using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace versioning.version
{
    internal static class Extension
    {
        public static string ToString2(this Version version)
        {
            return $"{version.Major}.{version.Minor}";
        }

        public static string ToString3(this Version version)
        {
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        public static string ToString4(this Version version)
        {
            if (version.Revision != -1)
                return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            else
                return version.ToString3();
        }
        
        public static string ToString4c(this Version version)
        {
            if (version.Revision != -1)
                return $"{version.Major},{version.Minor},{version.Build},{version.Revision}";
            else
                return version.ToString3();
        }
    }
}
