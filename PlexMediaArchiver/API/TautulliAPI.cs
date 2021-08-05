using Newtonsoft.Json;
using PlexMediaArchiver.Tautulli;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlexMediaArchiver.API
{
    public class TautulliAPI
    {
        private string tautulliUrl;
        //In Tautulli Settings|Web Interface|API Key (near the bottom of the page)
        private string apiKey;

        public TautulliAPI(Plex.Api.Models.Server.Server plexServer)
        {
            var plexURL = plexServer.FullUri.ToString();

            SetFullURLAndAPIKey(plexURL.Substring(0, plexURL.LastIndexOf(":")));
        }

        public TautulliAPI(string tautulliBaseServerUrl)
        {
            SetFullURLAndAPIKey(tautulliBaseServerUrl);
        }

        private void SetFullURLAndAPIKey(string baseURL)
        {
            var port = ConfigurationManager.AppSettings["TautulliPort"] ?? "";

            baseURL = baseURL.Trim('/');

            tautulliUrl = $"{(!baseURL.Contains("http") ? "http://" : "")}{baseURL}:{port}";

            apiKey = ConfigurationManager.AppSettings["TautulliAPIKey"] ?? "";
        }

        private T deserializeJSON<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            });
        }

        private void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }

        private T doAPIRequest<T>(string resource, object req = null, Method method = Method.GET)
        {
            return doRestRequest<T>(tautulliUrl.ToString(), resource, req, method);
        }

        private T doRestRequest<T>(string baseUrl, string resource, object req, Method method = Method.GET)
        {
            var url = $"{baseUrl}/api/v2?apikey={apiKey}&cmd={resource}";

            if (req != null)
            {
                foreach (PropertyInfo propertyInfo in req.GetType().GetProperties())
                {
                    var value = propertyInfo.GetValue(req, null);

                    if (value != null)
                    {
                        url += "&" + propertyInfo.Name + "=" + value.ToString();
                    }
                }
            }

            var client = new RestClient(url);
            var request = new RestRequest(method);

            request.JsonSerializer = new RestSharp.Serializers.NewtonsoftJson.JsonNetSerializer();

            IRestResponse response = client.Execute(request);

            return deserializeJSON<T>(response.Content);
        }

        public List<Library> GetLibraries()
        {
            return doAPIRequest<Tautulli.GetLibraries>("get_libraries").Response.Data;
        }

        public List<User> GetUsers()
        {
            return doAPIRequest<Tautulli.GetUsers>("get_users").Response.Data;
        }

        public User GetUser(string ID, bool includeLastSeen = true)
        {
            return doAPIRequest<User>("get_user", new { user_id = ID, include_last_seen = includeLastSeen }, Method.POST);
        }

        public List<MediaInfoData> GetMetadata(string ratingKey)
        {
            return doAPIRequest<Tautulli.GetMetadata>("get_metadata", new { rating_key = ratingKey }, Method.POST).Response.Data.MediaInfo;
        }

        private Library GetLibrary(string name)
        {
            return GetLibraries().FirstOrDefault(l => l.SectionName == name);
        }

        public Library GetMovieLibrary()
        {
            return GetLibrary("Movies");
        }

        public Library GetTVLibrary()
        {
            return GetLibrary("TV Shows");
        }

        public MediaInfo GetLibraryMediaInfoByRatingKey(string ratingKey = null, string sectionType = "movie", string orderColumn = "last_played",
                                                        string orderDir = null, int start = 0, int length = 25, string search = null)
        {
            return GetLibraryMediaInfo(ratingKey, null, sectionType, orderColumn, orderDir, start, length, search);
        }

        public MediaInfo GetLibraryMediaInfoBySectionID(string sectionID, string sectionType = "movie", string orderColumn = "last_played",
                                                        string orderDir = null, int start = 0, int length = 25, string search = null, bool loadDetailedMetaData = false)
        {
            return GetLibraryMediaInfo(null, sectionID, sectionType, orderColumn, orderDir, start, length, search, loadDetailedMetaData);
        }

        private MediaInfo GetLibraryMediaInfo(string ratingKey, string sectionID, string sectionType, string orderColumn, string orderDir, int start, int length, string search,
            bool loadDetailedMetaData = false)
        {
            var resp = doAPIRequest<Tautulli.GetLibraryMediaInfo>("get_library_media_info", new
            {
                section_id = sectionID,
                rating_key = ratingKey,
                section_type = sectionType,
                order_column = orderColumn,
                order_dir = orderDir,
                start,
                length,
                search
            }, Method.POST).Response.Data;

            if (loadDetailedMetaData)
            {
                foreach (var media in resp.Data)
                {
                    media.DetailedMetaData = GetMetadata(media.rating_key);
                }
            }

            return resp;
        }
    }
}
