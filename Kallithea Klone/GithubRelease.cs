﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kallithea_Klone
{
    class GithubRelease
    {
        [JsonProperty("tag_name")]
        public string Tag { get; set; }

        [JsonProperty("draft")]
        public bool IsDraft { get; set; }

        [JsonProperty("assets")]
        public Asset[] Assets { get; set; }

        public class Asset
        {
            [JsonProperty("browser_download_url")]
            public string URL { get; set; }
        }
    }
}
