using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kallithea_Klone
{
    class Location
    {
        public string Name { get; set; }

        public List<Location> InnerLocations { get; set; }

        public List<string> Repositories { get; set; }

        public Location(string name = null)
        {
            Name = name ?? "";
            InnerLocations = new List<Location>();
            Repositories = new List<string>();
        }
    }
}
