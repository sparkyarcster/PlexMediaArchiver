using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Tautulli
{
    public class MediaInfoData
    {
        [JsonProperty("added_at", NullValueHandling = NullValueHandling.Ignore)]
        public int? added_at { get; set; }
        [JsonProperty("audio_channels", NullValueHandling = NullValueHandling.Ignore)]
        public string audio_channels { get; set; }
        [JsonProperty("audio_codec", NullValueHandling = NullValueHandling.Ignore)]
        public string audio_codec { get; set; }
        [JsonProperty("bitrate", NullValueHandling = NullValueHandling.Ignore)]
        public string bitrate { get; set; }
        [JsonProperty("container", NullValueHandling = NullValueHandling.Ignore)]
        public string container { get; set; }
        [JsonProperty("file_size", NullValueHandling = NullValueHandling.Ignore)]
        public int? file_size { get; set; }
        [JsonProperty("grandparent_rating_key", NullValueHandling = NullValueHandling.Ignore)]
        public string grandparent_rating_key { get; set; }
        [JsonProperty("last_played", NullValueHandling = NullValueHandling.Ignore)]
        public int? last_played { get; set; }
        [JsonProperty("media_index", NullValueHandling = NullValueHandling.Ignore)]
        public string media_index { get; set; }
        [JsonProperty("media_type", NullValueHandling = NullValueHandling.Ignore)]
        public string media_type { get; set; }
        [JsonProperty("parent_media_index", NullValueHandling = NullValueHandling.Ignore)]
        public string parent_media_index { get; set; }
        [JsonProperty("parent_rating_key", NullValueHandling = NullValueHandling.Ignore)]
        public string parent_rating_key { get; set; }
        [JsonProperty("play_count", NullValueHandling = NullValueHandling.Ignore)]
        public int? play_count { get; set; }
        [JsonProperty("rating_key", NullValueHandling = NullValueHandling.Ignore)]
        public string rating_key { get; set; }
        [JsonProperty("section_id", NullValueHandling = NullValueHandling.Ignore)]
        public string section_id { get; set; }
        [JsonProperty("section_type", NullValueHandling = NullValueHandling.Ignore)]
        public string section_type { get; set; }
        [JsonProperty("sort_title", NullValueHandling = NullValueHandling.Ignore)]
        public string sort_title { get; set; }
        [JsonProperty("thumb", NullValueHandling = NullValueHandling.Ignore)]
        public string thumb { get; set; }
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string title { get; set; }
        [JsonProperty("video_codec", NullValueHandling = NullValueHandling.Ignore)]
        public string video_codec { get; set; }
        [JsonProperty("video_framerate", NullValueHandling = NullValueHandling.Ignore)]
        public string video_framerate { get; set; }
        [JsonProperty("video_resolution", NullValueHandling = NullValueHandling.Ignore)]
        public string video_resolution { get; set; }
        [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
        public string year { get; set; }
        public List<MediaInfoData> DetailedMetaData { get; set; }
    }
}
