using System.Collections.Generic;

namespace Kallithea_Klone.Other_Classes
{
    public class Location
    {
        //  Variables
        //  =========

        public string Name { get; set; }

        public List<Location> InnerLocations { get; set; }

        public List<string> Repositories { get; set; }

        //  Constructors
        //  ============

        public Location(string name = null)
        {
            Name = name ?? "";
            InnerLocations = new List<Location>();
            Repositories = new List<string>();
        }
    }
}
