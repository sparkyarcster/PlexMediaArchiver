using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Tautulli
{
    public class Libraries : BaseResponse
    {
        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public List<Library> Data { get; set; }
    }
}
