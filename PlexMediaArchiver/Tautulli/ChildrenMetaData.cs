using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Tautulli
{
    public class ChildrenMetaData
    {
        [JsonProperty("children_list", NullValueHandling = NullValueHandling.Ignore)]
        public List<MediaInfoData> ChildrenList { get; set; }

        [JsonProperty("children_count", NullValueHandling = NullValueHandling.Ignore)]
        public int ChildrenCount { get; set; }
    }
}
