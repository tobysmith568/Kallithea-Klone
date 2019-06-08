using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.Models.Repositories
{
    public class Repository : IRepository
    {
        //  Properties
        //  ==========

        [JsonProperty("repo_type")]
        public string Type { get; set; }

        [JsonProperty("repo_name")]
        public string URL { get; set; }
    }
}
