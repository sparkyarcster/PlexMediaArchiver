using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Tautulli
{
    public class Library
    {
        [JsonProperty("art", NullValueHandling = NullValueHandling.Ignore)]
        public string Art { get; set; }
        [JsonProperty("child_count", NullValueHandling = NullValueHandling.Ignore)]
        public string ChildCount { get; set; }
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public int Count { get; set; }
        [JsonProperty("is_active", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsActive { get; set; }
        [JsonProperty("parent_count", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentCount { get; set; }
        [JsonProperty("section_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SectionID { get; set; }
        [JsonProperty("section_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SectionName { get; set; }
        [JsonProperty("section_type", NullValueHandling = NullValueHandling.Ignore)]
        public string SectionType { get; set; }
        [JsonProperty("thumb", NullValueHandling = NullValueHandling.Ignore)]
        public string Thumb { get; set; }
    }
}
