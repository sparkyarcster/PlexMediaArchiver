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
            var port = Classes.Constants.TautulliPort;

            baseURL = baseURL.Trim('/');

            tautulliUrl = $"{(!baseURL.Contains("http") ? "http://" : "")}{baseURL}:{port}";

            apiKey = Classes.Constants.TautulliAPIKey;
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
            var resp = doAPIRequest<Tautulli.GetLibraries>("get_libraries");

            if (!string.IsNullOrEmpty(resp.Response.Error))
            {
                Classes.AppLogger.log.Error($"GetLibraries: {resp.Response.Error}");
            }

            return resp.Response.Data;
        }

        public List<User> GetUsers()
        {
            var resp = doAPIRequest<Tautulli.GetUsers>("get_users");

            if (!string.IsNullOrEmpty(resp.Response.Error))
            {
                Classes.AppLogger.log.Error($"GetUsers: {resp.Response.Error}");
            }

            return resp.Response.Data;
        }

        public User GetUser(string ID, bool includeLastSeen = true)
        {
            return doAPIRequest<User>("get_user", new { user_id = ID, include_last_seen = includeLastSeen });
        }

        public MetaData GetMetadata(string ratingKey)
        {
            var resp = doAPIRequest<Tautulli.GetMetadata>("get_metadata", new { rating_key = ratingKey });

            if (!string.IsNullOrEmpty(resp.Response.Message))
            {
                Classes.AppLogger.log.Error($"GetMetadata: {ratingKey}: {resp.Response.Message}");
            }

            return resp.Response.Data;
        }

        public ChildrenMetaData GetChildrenMetadata(string ratingKey, string mediaType)
        {
            var resp = doAPIRequest<Tautulli.GetChildrenMetadata>("get_children_metadata", new { rating_key = ratingKey, media_type = mediaType });

            if (!string.IsNullOrEmpty(resp.Response.Message))
            {
                Classes.AppLogger.log.Error($"GetChildrenMetadata: {ratingKey}, {mediaType}: {resp.Response.Message}");
            }

            return resp.Response.Data;
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
                                                        string orderDir = null, int start = 0, int length = 25, string search = null, int updateInterval = 25)
        {
            return GetLibraryMediaInfo(ratingKey, null, sectionType, orderColumn, orderDir, start, length, search, updateInterval: updateInterval);
        }

        public MediaInfo GetLibraryMediaInfoBySectionID(string sectionID, string sectionType = "movie", string orderColumn = "last_played",
                                                        string orderDir = null, int start = 0, int length = 25, string search = null, bool loadDetailedMetaData = false,
                                                        int updateInterval = 25)
        {
            return GetLibraryMediaInfo(null, sectionID, sectionType, orderColumn, orderDir, start, length, search, loadDetailedMetaData, updateInterval);
        }

        private MediaInfo GetLibraryMediaInfo(string ratingKey, string sectionID, string sectionType, string orderColumn, string orderDir, int start, int length, string search,
            bool loadDetailedMetaData = false, int updateInterval = 25)
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
                search,
                refresh = true
            });

            if (!string.IsNullOrEmpty(resp.Response.Error))
            {
                Classes.AppLogger.log.Error($"GetLibraryMediaInfo: {resp.Response.Error}");
            }

            if (loadDetailedMetaData)
            {
                Classes.AppLogger.log.Info("Loading detailed metadata, this may take a while...");
                var counter = 0;
                var totalCount = resp.Response.Data.Data.Count();

                foreach (var media in resp.Response.Data.Data)
                {
                    media.DetailedMetaData = DoGetMetaData(media.rating_key);

                    if (media.DetailedMetaData.MediaInfo == null || (media.DetailedMetaData.MediaInfo != null && media.DetailedMetaData.MediaInfo.Count == 0))
                    {
                        var mediaInfo = new MediaInfo();
                        var mediaInfoData = new MediaInfoData();

                        mediaInfoData.Parts = GetChildDataParts(media.rating_key, media.media_type);
                        
                        if ((media.file_size ?? 0) == 0)
                        {
                            media.file_size = mediaInfoData.Parts.Sum(p => p.file_size ?? 0);
                        }

                        mediaInfo.Data = new List<MediaInfoData>();
                        mediaInfo.Data.Add(mediaInfoData);
                    }

                    counter++;

                    if (counter % updateInterval == 0)
                    {
                        Classes.AppLogger.log.Info($"{counter} / {totalCount}");
                    }

                    if (counter % 100 == 0)
                    {
                        Classes.AppLogger.log.Info("Let the service catch up for a moment...");
                        System.Threading.Thread.Sleep(3000);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }

            return resp.Response.Data;
        }

        private MetaData DoGetMetaData(string rating_key, int retry = 0)
        {
            MetaData metaData = null;

            try
            {
                metaData = GetMetadata(rating_key);

                if (metaData == null)
                {
                    throw new Exception($"Metadata for {rating_key} was null.");
                }
            }
            catch (System.Exception ex)
            {
                if (retry < 3)
                {
                    return DoGetMetaData(rating_key, retry + 1);
                }
                else
                {
                    Classes.AppLogger.log.Error(ex, $"Issue getting metadata for {rating_key}.");
                }
            }

            return metaData;
        }

        private List<Parts> GetChildDataParts(string rating_key, string media_type)
        {
            var childMediaInfoParts = new List<Parts>();
            var childrenMetaData = DoGetChildrenMetaData(rating_key, media_type);

            if (childrenMetaData != null && childrenMetaData.ChildrenList.Any())
            {
                foreach (var child in childrenMetaData.ChildrenList.Where(c => !string.IsNullOrWhiteSpace(c.rating_key)))
                {
                    var childMetaData = DoGetMetaData(child.rating_key);

                    if (childMetaData.MediaInfo == null || (childMetaData.MediaInfo != null && childMetaData.MediaInfo.Count == 0))
                    {
                        childMediaInfoParts.AddRange(GetChildDataParts(childMetaData.RatingKey, childMetaData.MediaType));
                    }
                    else
                    {
                        foreach (var info in childMetaData.MediaInfo)
                        {
                            if (info.Parts != null && info.Parts.Any())
                            {
                                childMediaInfoParts.AddRange(info.Parts);
                            }
                        }
                    }
                }
            }

            return childMediaInfoParts;
        }

        private ChildrenMetaData DoGetChildrenMetaData(string rating_key, string mediaType, int retry = 0)
        {
            ChildrenMetaData childrenMetaData = null;

            try
            {
                childrenMetaData = GetChildrenMetadata(rating_key, mediaType);

                if (childrenMetaData == null)
                {
                    throw new Exception($"Children metadata for {rating_key} was null.");
                }
            }
            catch (System.Exception ex)
            {
                if (retry < 3)
                {
                    return DoGetChildrenMetaData(rating_key, mediaType, retry + 1);
                }
                else
                {
                    Classes.AppLogger.log.Error(ex, $"Issue getting children metadata for {rating_key}.");
                }
            }

            return childrenMetaData;
        }
    }
}
