using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Tautulli
{
    public class GetMetadata
    {
        [JsonProperty("response", NullValueHandling = NullValueHandling.Ignore)]
        public MediaMetadata Response { get; set; }
    }
}
