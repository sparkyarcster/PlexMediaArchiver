using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Tautulli
{
    public class User
    {
        [JsonProperty("allow_guest", NullValueHandling = NullValueHandling.Ignore)]
        public bool AllowGuest { get; set; }
        [JsonProperty("do_notify", NullValueHandling = NullValueHandling.Ignore)]
        public bool DoNotify { get; set; }
        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }
        [JsonProperty("filter_all", NullValueHandling = NullValueHandling.Ignore)]
        public string FilterAll { get; set; }
        [JsonProperty("filter_movies", NullValueHandling = NullValueHandling.Ignore)]
        public string FilterMovies { get; set; }
        [JsonProperty("filter_music", NullValueHandling = NullValueHandling.Ignore)]
        public string FilterMusic { get; set; }
        [JsonProperty("filter_photos", NullValueHandling = NullValueHandling.Ignore)]
        public string FilterPhotos { get; set; }
        [JsonProperty("filter_tv", NullValueHandling = NullValueHandling.Ignore)]
        public string FilterTV { get; set; }
        [JsonProperty("is_active", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsActive { get; set; }
        [JsonProperty("is_admin", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsAdmin { get; set; }
        [JsonProperty("is_allow_sync", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsAllowSync { get; set; }
        [JsonProperty("is_home_user", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsHomeUser { get; set; }
        [JsonProperty("is_restricted", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsRestricted { get; set; }
        [JsonProperty("keep_history", NullValueHandling = NullValueHandling.Ignore)]
        public bool KeppHistory { get; set; }
        [JsonProperty("row_id", NullValueHandling = NullValueHandling.Ignore)]
        public int RowID { get; set; }
        [JsonProperty("server_token", NullValueHandling = NullValueHandling.Ignore)]
        public string ServerToken { get; set; }
        [JsonProperty("shared_libraries", NullValueHandling = NullValueHandling.Ignore)]
        public string[] SharedLibraries { get; set; }
        [JsonProperty("thumb", NullValueHandling = NullValueHandling.Ignore)]
        public string Thumb { get; set; }
        [JsonProperty("user_thumb", NullValueHandling = NullValueHandling.Ignore)]
        public string UserThumb { get; set; }
        [JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
        public int UserID { get; set; }
        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string UserName { get; set; }
        [JsonProperty("friendly_name", NullValueHandling = NullValueHandling.Ignore)]
        public string FriendlyName { get; set; }
        [JsonProperty("last_seen", NullValueHandling = NullValueHandling.Ignore)]
        public int? LastSeen { get; set; }
        [JsonProperty("deleted_user", NullValueHandling = NullValueHandling.Ignore)]
        public string DeletedUser { get; set; }
    }
}
