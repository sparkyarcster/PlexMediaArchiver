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

        public List<PMAData.Model.Movie> GetMovies()
        {
            return Classes.Database.MovieRepository.GetMovies();
        }

        public List<PMAData.Model.Movie> GetMovies(Classes.Enum.MediaCodec mediaCodec)
        {
            return Classes.Database.MovieRepository.GetMoviesByCodec(mediaCodec.ToString());
        }

        public List<PMAData.Model.Movie> GetMovies(Classes.Enum.Container container)
        {
            return Classes.Database.MovieRepository.GetMoviesByContainer(container.ToString());
        }

        public List<PMAData.Model.TVShow> GetTVShows()
        {
            return Classes.Database.TVShowRepository.GetTVShows();
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
                var movies = tautulliAPI.GetLibraryMediaInfoBySectionID(movieLibrary.SectionID, sectionType: movieLibrary.SectionType, length: movieLibrary.Count * 2, loadDetailedMetaData: true, updateInterval: 25);
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
                var tvshows = tautulliAPI.GetLibraryMediaInfoBySectionID(tvLibraryLibrary.SectionID, sectionType: tvLibraryLibrary.SectionType, length: tvLibraryLibrary.Count * 2, loadDetailedMetaData: true, updateInterval: 5);
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
            var mediaItem = Classes.Mapper.mapper.Map<PMAData.Model.Movie>(movie);

            mediaItem.GenericData.Add(new PMAData.Model.GenericData()
            {
                ID = Guid.NewGuid(),
                MediaID = mediaItem.ID,
                MediaType = "movie",
                DataKey = "Container",
                DataValue = movie.container
            });

            if (movie.DetailedMetaData != null && movie.DetailedMetaData.MediaInfo != null
                && movie.DetailedMetaData.MediaInfo.Any())
            {
                mediaItem.GenericData.Add(new PMAData.Model.GenericData()
                {
                    ID = Guid.NewGuid(),
                    MediaID = mediaItem.ID,
                    MediaType = "movie",
                    DataKey = "Video Codec",
                    DataValue = string.Join(",", movie.DetailedMetaData.MediaInfo.Select(m => m.video_codec))
                });

                var detailedMetaDataMediaInfo = movie.DetailedMetaData.MediaInfo.First();

                if (detailedMetaDataMediaInfo.Parts != null && detailedMetaDataMediaInfo.Parts.Any())
                {
                    //foreach (var part in detailedMetaDataMediaInfo.Parts)
                    //{
                    //    mediaItem.GenericData.Add(new PMAData.Model.GenericData()
                    //    {
                    //        ID = Guid.NewGuid(),
                    //        MediaID = mediaItem.ID,
                    //        MediaType = "movie",
                    //        DataKey = "File",
                    //        DataValue = part.file ?? ""
                    //    });
                    //}

                    foreach (var drive in detailedMetaDataMediaInfo.Parts.Select(p => p.drive).Distinct())
                    {
                        mediaItem.GenericData.Add(new PMAData.Model.GenericData()
                        {
                            ID = Guid.NewGuid(),
                            MediaID = mediaItem.ID,
                            MediaType = "movie",
                            DataKey = "Drive",
                            DataValue = drive ?? ""
                        });
                    }

                    mediaItem.isArchived = detailedMetaDataMediaInfo.Parts.Any(p => p.file.StartsWith(Classes.Constants.ArchiveFilePath));
                    mediaItem.isCurrent = detailedMetaDataMediaInfo.Parts.Any(p => p.file.StartsWith(Classes.Constants.MediaFilePath));
                }
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
            var mediaItem = Classes.Mapper.mapper.Map<PMAData.Model.TVShow>(tvshow);

            if (tvshow.DetailedMetaData != null)
            {
                mediaItem.GenericData.Add(new PMAData.Model.GenericData()
                {
                    ID = Guid.NewGuid(),
                    MediaID = mediaItem.ID,
                    MediaType = "tvshow",
                    DataKey = "Episodes",
                    DataValue = tvshow.DetailedMetaData.ChildrenCount.ToString()
                });

                if (tvshow.DetailedMetaData.MediaInfo != null && tvshow.DetailedMetaData.MediaInfo.Any())
                {
                    var detailedMetaDataMediaInfo = tvshow.DetailedMetaData.MediaInfo.First();

                    if (detailedMetaDataMediaInfo.Parts != null && detailedMetaDataMediaInfo.Parts.Any())
                    {
                        //foreach (var part in detailedMetaDataMediaInfo.Parts)
                        //{
                        //    mediaItem.GenericData.Add(new PMAData.Model.GenericData()
                        //    {
                        //        ID = Guid.NewGuid(),
                        //        MediaID = mediaItem.ID,
                        //        MediaType = "tvshow",
                        //        DataKey = "File",
                        //        DataValue = part.file ?? ""
                        //    });
                        //}

                        foreach (var drive in detailedMetaDataMediaInfo.Parts.Select(p => p.drive).Distinct())
                        {
                            mediaItem.GenericData.Add(new PMAData.Model.GenericData()
                            {
                                ID = Guid.NewGuid(),
                                MediaID = mediaItem.ID,
                                MediaType = "tvshow",
                                DataKey = "Drive",
                                DataValue = drive ?? ""
                            });
                        }

                        mediaItem.isArchived = detailedMetaDataMediaInfo.Parts.Any(p => p.file.StartsWith(Classes.Constants.ArchiveFilePath));
                        mediaItem.isCurrent = detailedMetaDataMediaInfo.Parts.Any(p => p.file.StartsWith(Classes.Constants.MediaFilePath));
                    }
                }
            }

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
