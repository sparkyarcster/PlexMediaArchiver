using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Tautulli
{
    public class BaseResponse
    {
        [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
        public string Result { get; set; }
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
    }
}
