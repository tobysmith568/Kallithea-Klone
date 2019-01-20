using System;
using System.Web;
using Newtonsoft.Json;

namespace Kallithea_Klone.Other_Classes
{
    public class URIPair
    {
        //  JSON Variables
        //  ==============
        
        private string remote;

        //  JSON Properties
        //  ===============

        [JsonProperty("Local")]
        public string Local { get; set; }

        [JsonProperty("Remote")]
        public string Remote
        {
            get => remote;
            set
            {
                Uri uri = new Uri(value);
                remote = $"{uri.Scheme}://{HttpUtility.UrlEncode(MainWindow.Username)}@{uri.Host}{uri.PathAndQuery}";
            }
        }

        //  Constructors
        //  ============

        public  URIPair(string local, string remote)
        {
            Local = local;
            Remote = remote;
        }
    }
}
