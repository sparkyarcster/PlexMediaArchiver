using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Tautulli
{
    public class GetChildrenMetadata
    {
        [JsonProperty("response", NullValueHandling = NullValueHandling.Ignore)]
        public MediaChildrenMetadata Response { get; set; }
    }
}
