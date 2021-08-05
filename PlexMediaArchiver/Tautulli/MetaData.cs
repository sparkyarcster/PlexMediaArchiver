using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Tautulli
{
    public class MetaData
    {
        [JsonProperty("media_info", NullValueHandling = NullValueHandling.Ignore)]
        public List<MediaInfoData> MediaInfo { get; set; }
    }
}
