using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kallithea_Klone.Other_Classes
{
    class User
    {
        //  JSON Properties
        //  ===============

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
