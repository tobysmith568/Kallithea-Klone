using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kallithea_Klone
{
    class User
    {
        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
