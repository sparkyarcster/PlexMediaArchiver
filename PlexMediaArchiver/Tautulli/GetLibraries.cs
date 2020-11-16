using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Tautulli
{
    public class GetLibraries
    {
        [JsonProperty("response", NullValueHandling = NullValueHandling.Ignore)]
        public Libraries Response { get; set; }
    }
}
