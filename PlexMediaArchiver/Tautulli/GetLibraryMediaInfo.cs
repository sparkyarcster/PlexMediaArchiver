using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Tautulli
{
    public class GetLibraryMediaInfo
    {
        [JsonProperty("response", NullValueHandling = NullValueHandling.Ignore)]
        public LibraryMediaInfo Response { get; set; }
    }
}
