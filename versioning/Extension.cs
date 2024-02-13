﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace versioning
{
    internal static class Extension
    {
        public static string ToString3(this Version version)
        {
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        public static string ToString4(this Version version)
        {
            if (version.Revision != -1)
                return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            else
                return ToString3(version);
        }
    }
}