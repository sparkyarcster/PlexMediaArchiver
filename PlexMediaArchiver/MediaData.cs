using System;
using PlexMediaArchiver.Classes.Model;
using PlexMediaArchiver.API;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PlexMediaArchiver
{
    public class MediaData
    {

        public MediaData()
        {
        }

        public List<PMAData.Model.Movie> GetMovies(Classes.Enum.MediaCodec? mediaCodec = null)
        {
            if (mediaCodec.HasValue)
            {
                return Classes.Database.MovieRepository.GetMoviesByCodec(mediaCodec.ToString());
            }
            else
            {
                return Classes.Database.MovieRepository.GetMovies();
            }
        }

        public void LoadData()
        {
            Classes.AppLogger.log.Info("Getting server information...");

            var plexAPI = new PlexAPI();
            var plexServer = plexAPI.GetPreferredServer();

            if (plexServer != null)
            {
                var tautulliAPI = new TautulliAPI(plexServer);

                Classes.AppLogger.log.Info("Getting Movies...");

                var movieLibrary = tautulliAPI.GetMovieLibrary();
                var movies = tautulliAPI.GetLibraryMediaInfoBySectionID(movieLibrary.SectionID, sectionType: movieLibrary.SectionType, length: movieLibrary.Count * 2, loadDetailedMetaData: false);
                var itemCounter = 0;
                var totalCount = movies.Data.Count();

                Classes.AppLogger.log.Info("Saving Movie Data...");

                foreach (var movie in movies.Data)
                {
                    IndexMovie(movie);

                    itemCounter++;

                    if (itemCounter % 100 == 0)
                    {
                        Classes.AppLogger.log.Info($"{itemCounter} / {totalCount}");
                    }
                }

                Classes.AppLogger.log.Info("Getting TV Shows...");

                var tvLibraryLibrary = tautulliAPI.GetTVLibrary();
                var tvshows = tautulliAPI.GetLibraryMediaInfoBySectionID(tvLibraryLibrary.SectionID, sectionType: tvLibraryLibrary.SectionType, length: tvLibraryLibrary.Count * 2);
                itemCounter = 0;
                totalCount = tvshows.Data.Count();

                Classes.AppLogger.log.Info("Saving TV Show Data...");

                foreach (var tvshow in tvshows.Data)
                {
                    IndexTVShow(tvshow);

                    itemCounter++;

                    if (itemCounter % 25 == 0)
                    {
                        Classes.AppLogger.log.Info($"{itemCounter} / {totalCount}");
                    }
                }

                Classes.AppLogger.log.Info("Getting Users...");

                var users = tautulliAPI.GetUsers();
                itemCounter = 0;
                totalCount = users.Count();

                Classes.AppLogger.log.Info("Saving User Data...");

                foreach (var user in users)
                {
                    var userData = tautulliAPI.GetUser(user.UserID.ToString());

                    IndexUser(user, userData);

                    itemCounter++;

                    if (itemCounter % 10 == 0)
                    {
                        Classes.AppLogger.log.Info($"{itemCounter} / {totalCount}");
                    }
                }
            }
            else
            {
                Classes.AppLogger.log.Error("Could not locate preferred Plex server.");
            }
        }

        private void IndexMovie(Tautulli.MediaInfoData movie)
        {
            var lastViewed = ConvertLastViewed(movie.last_played ?? 0);

            var mediaItem = new PMAData.Model.Movie()
            {
                ID = int.Parse(movie.rating_key),
                Title = movie.title,
                Year = movie.year,
                LastPlayed = lastViewed
            };

            if (movie.DetailedMetaData != null && movie.DetailedMetaData.Any())
            {
                mediaItem.GenericData.Add(new PMAData.Model.GenericData()
                {
                    ID = Guid.NewGuid(),
                    MediaID = mediaItem.ID,
                    MediaType = "movie",
                    DataKey = "Video Codec",
                    DataValue = string.Join(",", movie.DetailedMetaData.Select(m => m.video_codec))
                });
            }
            else
            {
                mediaItem.GenericData.Add(new PMAData.Model.GenericData()
                {
                    ID = Guid.NewGuid(),
                    MediaID = mediaItem.ID,
                    MediaType = "movie",
                    DataKey = "Video Codec",
                    DataValue = movie.video_codec
                });
            }

            Classes.Database.MovieRepository.CreateOrUpdate(mediaItem);
        }

        private void IndexTVShow(Tautulli.MediaInfoData tvshow)
        {
            var lastViewed = ConvertLastViewed(tvshow.last_played ?? 0);

            var mediaItem = new PMAData.Model.TVShow()
            {
                ID = int.Parse(tvshow.rating_key),
                Title = tvshow.title,
                Year = tvshow.year,
                LastPlayed = lastViewed
            };

            Classes.Database.TVShowRepository.CreateOrUpdate(mediaItem);
        }

        private void IndexUser(Tautulli.User userHeaders, Tautulli.User userData)
        {
            var lastSeen = ConvertLastViewed(userData.LastSeen ?? 0);

            var userItem = new PMAData.Model.User()
            {
                ID = userHeaders.UserID,
                UserName = userHeaders.UserName,
                LastActivity = lastSeen,
                LastLogin = null
            };

            Classes.Database.UserRepository.CreateOrUpdate(userItem);
        }

        private DateTime? ConvertLastViewed(int lastViewedAt)
        {
            DateTime? lastViewed = null;

            try
            {
                if (lastViewedAt > 0)
                {
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(lastViewedAt);
                    lastViewed = DateTime.SpecifyKind(dateTimeOffset.DateTime, DateTimeKind.Utc);
                }

                return lastViewed.HasValue ? lastViewed.Value.ToLocalTime() : (DateTime?)null;
            }
            catch (Exception ex)
            {
                Classes.AppLogger.log.Error(ex.Message);
                return null;
            }
        }
    }
}
