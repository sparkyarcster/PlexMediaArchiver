using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Tautulli
{
    public class Parts
    {
        [JsonProperty("file", NullValueHandling = NullValueHandling.Ignore)]
        public string file { get; set; }
        [JsonProperty("file_size", NullValueHandling = NullValueHandling.Ignore)]
        public long? file_size { get; set; }
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string id { get; set; }

        public string drive
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(file))
                {
                    return System.IO.Path.GetPathRoot(file);
                }
                else
                {
                    return "";
                }
            }
        }
        
        
        public Parts()
        {

        }
    }
}
