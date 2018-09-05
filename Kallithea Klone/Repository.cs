using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kallithea_Klone
{
    class Repository
    {
        //  JSON Properties
        //  ===============

        [JsonProperty("repo_name")]
        public string URL { get; set; }
    }
}
