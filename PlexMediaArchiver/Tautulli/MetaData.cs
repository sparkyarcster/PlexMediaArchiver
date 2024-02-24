using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Tautulli
{
    public class MetaData
    {
        [JsonProperty("media_type", NullValueHandling = NullValueHandling.Ignore)]
        public string MediaType { get; set; }

        [JsonProperty("section_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SectionId { get; set; }

        [JsonProperty("library_name", NullValueHandling = NullValueHandling.Ignore)]
        public string LibraryName { get; set; }

        [JsonProperty("rating_key", NullValueHandling = NullValueHandling.Ignore)]
        public string RatingKey { get; set; }

        [JsonProperty("parent_rating_key", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentRatingKey { get; set; }

        [JsonProperty("grandparent_rating_key", NullValueHandling = NullValueHandling.Ignore)]
        public string GrandparentRatingKey { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("parent_title", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentTitle { get; set; }

        [JsonProperty("grandparent_title", NullValueHandling = NullValueHandling.Ignore)]
        public string GrandparentTitle { get; set; }

        [JsonProperty("original_title", NullValueHandling = NullValueHandling.Ignore)]
        public string OriginalTitle { get; set; }

        [JsonProperty("sort_title", NullValueHandling = NullValueHandling.Ignore)]
        public string SortTitle { get; set; }

        [JsonProperty("media_index", NullValueHandling = NullValueHandling.Ignore)]
        public string MediaIndex { get; set; }

        [JsonProperty("parent_media_index", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentMediaIndex { get; set; }

        [JsonProperty("studio", NullValueHandling = NullValueHandling.Ignore)]
        public string Studio { get; set; }

        [JsonProperty("content_rating", NullValueHandling = NullValueHandling.Ignore)]
        public string ContentRating { get; set; }

        [JsonProperty("summary", NullValueHandling = NullValueHandling.Ignore)]
        public string Summary { get; set; }

        [JsonProperty("tagline", NullValueHandling = NullValueHandling.Ignore)]
        public string Tagline { get; set; }

        [JsonProperty("rating", NullValueHandling = NullValueHandling.Ignore)]
        public string Rating { get; set; }

        [JsonProperty("rating_image", NullValueHandling = NullValueHandling.Ignore)]
        public string RatingImage { get; set; }

        [JsonProperty("audience_rating", NullValueHandling = NullValueHandling.Ignore)]
        public string AudienceRating { get; set; }

        [JsonProperty("audience_rating_image", NullValueHandling = NullValueHandling.Ignore)]
        public string AudienceRatingImage { get; set; }

        [JsonProperty("user_rating", NullValueHandling = NullValueHandling.Ignore)]
        public string UserRating { get; set; }

        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public string Duration { get; set; }

        [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
        public string Year { get; set; }

        [JsonProperty("parent_year", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentYear { get; set; }

        [JsonProperty("grandparent_year", NullValueHandling = NullValueHandling.Ignore)]
        public string GrandparentYear { get; set; }

        [JsonProperty("thumb", NullValueHandling = NullValueHandling.Ignore)]
        public string Thumb { get; set; }

        [JsonProperty("parent_thumb", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentThumb { get; set; }

        [JsonProperty("grandparent_thumb", NullValueHandling = NullValueHandling.Ignore)]
        public string GrandparentThumb { get; set; }

        [JsonProperty("art", NullValueHandling = NullValueHandling.Ignore)]
        public string Art { get; set; }

        [JsonProperty("banner", NullValueHandling = NullValueHandling.Ignore)]
        public string Banner { get; set; }

        [JsonProperty("originally_available_at", NullValueHandling = NullValueHandling.Ignore)]
        public string OriginallyAvailableAt { get; set; }

        [JsonProperty("added_at", NullValueHandling = NullValueHandling.Ignore)]
        public string AddedAt { get; set; }

        [JsonProperty("updated_at", NullValueHandling = NullValueHandling.Ignore)]
        public string UpdatedAt { get; set; }

        [JsonProperty("last_viewed_at", NullValueHandling = NullValueHandling.Ignore)]
        public string LastViewedAt { get; set; }

        [JsonProperty("guid", NullValueHandling = NullValueHandling.Ignore)]
        public string Guid { get; set; }

        [JsonProperty("parent_guid", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentGuid { get; set; }

        [JsonProperty("grandparent_guid", NullValueHandling = NullValueHandling.Ignore)]
        public string GrandparentGuid { get; set; }

        [JsonProperty("directors", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Directors { get; set; }

        [JsonProperty("writers", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Writers { get; set; }

        [JsonProperty("actors", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Actors { get; set; }

        [JsonProperty("genres", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Genres { get; set; }

        [JsonProperty("labels", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Labels { get; set; }

        [JsonProperty("collections", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Collections { get; set; }

        [JsonProperty("guids", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Guids { get; set; }

        [JsonProperty("parent_guids", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ParentGuids { get; set; }

        [JsonProperty("grandparent_guids", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> GrandparentGuids { get; set; }

        [JsonProperty("full_title", NullValueHandling = NullValueHandling.Ignore)]
        public string FullTitle { get; set; }

        [JsonProperty("children_count", NullValueHandling = NullValueHandling.Ignore)]
        public int ChildrenCount { get; set; }

        [JsonProperty("live", NullValueHandling = NullValueHandling.Ignore)]
        public int Live { get; set; }

        [JsonProperty("media_info", NullValueHandling = NullValueHandling.Ignore)]
        public List<MediaInfoData> MediaInfo { get; set; }
    }
}
