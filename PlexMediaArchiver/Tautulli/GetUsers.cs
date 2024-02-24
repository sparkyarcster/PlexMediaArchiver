using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Tautulli
{
    public class GetUsers
    {
        [JsonProperty("response", NullValueHandling = NullValueHandling.Ignore)]
        public Users Response { get; set; }
    }
}
