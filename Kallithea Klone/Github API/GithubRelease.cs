﻿using Newtonsoft.Json;

namespace Kallithea_Klone.Github_API
{
    public class GithubRelease
    {
        //  JSON Properties
        //  ===============

        [JsonProperty("html_url")]
        public string URL { get; set; }

        [JsonProperty("tag_name")]
        public string Tag { get; set; }

        [JsonProperty("draft")]
        public bool IsDraft { get; set; }

        [JsonProperty("assets")]
        public Asset[] Assets { get; set; }
    }

    public class Asset
    {
        //  JSON Properties
        //  ===============

        [JsonProperty("browser_download_url")]
        public string URL { get; set; }
    }
}
