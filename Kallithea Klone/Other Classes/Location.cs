using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kallithea_Klone.Other_Classes
{
    public class Location
    {
        //  Properties
        //  ==========

        public string Name
        {
            get
            {
                return Path.GetFileName(URL);
            }
        }
        public string URL { get; }
        public bool IsChecked { get; set; }

        //  Constructors
        //  ============

        public Location(string url)
        {            
            if (!Uri.IsWellFormedUriString(url ?? "", UriKind.RelativeOrAbsolute))
            {
                throw new ArgumentException("url is not a well-formed URL");
            }

            URL = url;
        }

        //  Overrides
        //  =========

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
