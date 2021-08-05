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
            Console.WriteLine("Getting server information...");

            var plexAPI = new PlexAPI();
            var plexServer = plexAPI.GetPreferredServer();

            if (plexServer != null)
            {
                var tautulliAPI = new TautulliAPI(plexServer);

                Console.WriteLine("Getting Movies...");

                var movieLibrary = tautulliAPI.GetMovieLibrary();
                var movies = tautulliAPI.GetLibraryMediaInfoBySectionID(movieLibrary.SectionID, sectionType: movieLibrary.SectionType, length: movieLibrary.Count, loadDetailedMetaData: true);
                var taskList = new List<Action>();

                foreach (var movie in movies.Data)
                {
                    taskList.Add(() =>
                    {
                        IndexMovie(movie);
                    });                    
                }

                Parallel.Invoke(new ParallelOptions() { MaxDegreeOfParallelism = 25 }, taskList.ToArray());

                Console.WriteLine("Getting TV Shows...");

                var tvLibraryLibrary = tautulliAPI.GetTVLibrary();
                var tvshows = tautulliAPI.GetLibraryMediaInfoBySectionID(tvLibraryLibrary.SectionID, sectionType: tvLibraryLibrary.SectionType, length: tvLibraryLibrary.Count);
                taskList = new List<Action>();

                foreach (var tvshow in tvshows.Data)
                {
                    taskList.Add(() =>
                    {
                        IndexTVShow(tvshow);
                    });
                }

                Parallel.Invoke(new ParallelOptions() { MaxDegreeOfParallelism = 15 }, taskList.ToArray());

                Console.WriteLine("Getting Users...");

                var users = tautulliAPI.GetUsers();
                taskList = new List<Action>();

                foreach (var user in users)
                {
                    var userData = tautulliAPI.GetUser(user.UserID.ToString());

                    taskList.Add(() =>
                    {
                        IndexUser(user, userData);
                    });
                }

                Parallel.Invoke(new ParallelOptions() { MaxDegreeOfParallelism = 5 }, taskList.ToArray());
            }
            else
            {
                Console.WriteLine("Could not locate preferred Plex server.");
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

            if (movie.DetailedMetaData.Any())
            {
                mediaItem.GenericData.Add(new PMAData.Model.GenericData()
                {
                    ID = Guid.NewGuid(),
                    MediaID = mediaItem.ID,
                    MediaType = "movie",
                    DataKey = "Video Codec",
                    DataValue = movie.DetailedMetaData.First().video_codec
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
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
