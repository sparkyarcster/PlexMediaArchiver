using System;
using System.Collections.Generic;
using System.Linq;

namespace PlexMediaArchiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Classes.AppLogger.Setup();

            bool resetDatabase = Classes.Constants.RefreshDatabase;

            if (resetDatabase)
            {
                Console.WriteLine("Are you sure you want to reset the database? Y/N");

                var confirm = Console.ReadKey();

                Console.WriteLine("");

                if (confirm.Key == ConsoleKey.Y)
                {
                    Classes.Database.Reset();
                }
                else
                {
                    resetDatabase = false;
                }
            }

            Classes.Database.Setup();

            var mediaData = new MediaData();

            if (resetDatabase)
            {
                mediaData.LoadData();
            }

            Classes.AppLogger.log.Info("Running reports...");

            List<string> reportsMade = new List<string>();
            var reportBuilder = new ReportBuilder();

            var hevcData = mediaData.GetMovies(Classes.Enum.MediaCodec.hevc).Select(m => new
            {
                Movie = m.Title,
                Year = m.Year,
                MediaSize = m.FileSize,
                Codec = m.GenericData.FirstOrDefault(d => d.DataKey == "Video Codec").DataValue,
                Container = m.GenericData.FirstOrDefault(d => d.DataKey == "Container").DataValue
            });

            if (hevcData.Count() > 0)
            {
                reportsMade.Add(reportBuilder.BuildReport(hevcData.OrderBy(m => m.Movie).ThenBy(m => m.Year).ToList(), "hevc"));
                hevcData = null;
            }

            var mpeg4Data = mediaData.GetMovies(Classes.Enum.MediaCodec.mpeg4).Select(m => new
            {
                Movie = m.Title,
                Year = m.Year,
                MediaSize = m.FileSize,
                Codec = m.GenericData.FirstOrDefault(d => d.DataKey == "Video Codec").DataValue,
                Container = m.GenericData.FirstOrDefault(d => d.DataKey == "Container").DataValue
            });

            if (mpeg4Data.Count() > 0)
            {
                reportsMade.Add(reportBuilder.BuildReport(mpeg4Data.OrderBy(m => m.Movie).ThenBy(m => m.Year).ToList(), "mpeg4"));
                mpeg4Data = null;
            }

            var aviData = mediaData.GetMovies(Classes.Enum.Container.avi).Select(m => new
            {
                Movie = m.Title,
                Year = m.Year,
                MediaSize = m.FileSize,
                Codec = m.GenericData.FirstOrDefault(d => d.DataKey == "Video Codec").DataValue,
                Container = m.GenericData.FirstOrDefault(d => d.DataKey == "Container").DataValue
            });

            if (aviData.Count() > 0)
            {
                reportsMade.Add(reportBuilder.BuildReport(aviData.OrderBy(m => m.Movie).ThenBy(m => m.Year).ToList(), "avi"));
                aviData = null;
            }

            var movieIgnoreDate = DateTime.Now.AddDays(Classes.Constants.IgnoreMoviesLessThanDaysOld * -1);
            var movieCompareDate = DateTime.Now.AddDays(Classes.Constants.MovieAge * -1);

            var notWatchedMovies = mediaData.GetMovies()
                .Where(m => ((m.Added.HasValue && m.Added.Value < movieIgnoreDate) || (!m.Added.HasValue))
                    && (!m.LastPlayed.HasValue || m.LastPlayed.Value < movieCompareDate))
                .Select(m => new
            {
                Movie = m.Title,
                Year = m.Year,
                MediaSize = m.FileSize,
                DateAdded = m.Added,
                LastPlayed = m.LastPlayed
            });

            if (notWatchedMovies.Count() > 0)
            {
                reportsMade.Add(reportBuilder.BuildReport(notWatchedMovies.OrderBy(m => m.Movie).ThenBy(m => m.Year).ToList(), "movies_stale"));
                notWatchedMovies = null;
            }

            var tvshowIgnoreDate = DateTime.Now.AddDays(Classes.Constants.IgnoreTVShowsLessThanDaysOld * -1);
            var tvshowCompareDate = DateTime.Now.AddDays(Classes.Constants.TVAge * -1);

            var notWatchedTVShows = mediaData.GetTVShows()
                .Where(s=> ((s.Added.HasValue && s.Added.Value < tvshowIgnoreDate) || (!s.Added.HasValue))
                    && (!s.LastPlayed.HasValue || s.LastPlayed.Value < tvshowCompareDate))
                .Select(s => new
                {
                    TVShow = s.Title,
                    Year = s.Year,
                    MediaSize = s.FileSize,
                    DateAdded = s.Added,
                    LastPlayed = s.LastPlayed
                });

            if (notWatchedTVShows.Count() > 0)
            {
                reportsMade.Add(reportBuilder.BuildReport(notWatchedTVShows.OrderBy(s => s.TVShow).ThenBy(s => s.Year).ToList(), "tvshows_stale"));
                notWatchedTVShows = null;
            }

            if (!string.IsNullOrEmpty(Classes.Constants.ReportLocation) && reportsMade.Count() > 0)
            {
                try
                {
                    var reportPath = System.IO.Path.Combine(Classes.Constants.ReportLocation, DateTime.Now.ToString("yyyyMMdd"));

                    if (!System.IO.Directory.Exists(reportPath))
                    {
                        System.IO.Directory.CreateDirectory(reportPath);
                    }

                    foreach (var report in reportsMade)
                    {
                        if (!string.IsNullOrEmpty(report))
                        {
                            var filename = System.IO.Path.GetFileName(report);
                            var newfilepath = System.IO.Path.Combine(reportPath, filename);

                            System.IO.File.Copy(report, newfilepath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Classes.AppLogger.log.Error(ex, "There was an error copying the reports.");
                }
            }
        }
    }
}
