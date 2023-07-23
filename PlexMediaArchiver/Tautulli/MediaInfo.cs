using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Tautulli
{
    public class MediaInfo
    {
        [JsonProperty("draw", NullValueHandling = NullValueHandling.Ignore)]
        public bool Draw { get; set; }
        [JsonProperty("recordsTotal", NullValueHandling = NullValueHandling.Ignore)]
        public int? RecordsTotal { get; set; }
        [JsonProperty("recordsFiltered", NullValueHandling = NullValueHandling.Ignore)]
        public int? RecordsFiltered { get; set; }
        [JsonProperty("filtered_file_size", NullValueHandling = NullValueHandling.Ignore)]
        public long? FilteredFileSize { get; set; }
        [JsonProperty("total_file_size", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalFileSize { get; set; }
        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public List<MediaInfoData> Data { get; set; }
    }
}
