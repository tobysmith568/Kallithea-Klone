using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kallithea_Klone.Other_Classes
{
    public class RepositoryData
    {
        public string Name { get; set; }
        public string URL { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}