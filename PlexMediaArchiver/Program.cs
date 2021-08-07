using System;
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

            var reportBuilder = new ReportBuilder();

            var hevcData = mediaData.GetMovies(Classes.Enum.MediaCodec.hevc).Select(m => new
            {
                Movie = m.Title,
                Year = m.Year,
                Codec = m.GenericData.FirstOrDefault(d => d.DataKey == "Video Codec").DataValue,
                Container = m.GenericData.FirstOrDefault(d => d.DataKey == "Container").DataValue
            });

            if (hevcData.Count() > 0)
            {
                reportBuilder.BuildReport(hevcData.OrderBy(m => m.Movie).ThenBy(m => m.Year).ToList(), "hevc");
                hevcData = null;
            }

            var mpeg4Data = mediaData.GetMovies(Classes.Enum.MediaCodec.mpeg4).Select(m => new
            {
                Movie = m.Title,
                Year = m.Year,
                Codec = m.GenericData.FirstOrDefault(d => d.DataKey == "Video Codec").DataValue,
                Container = m.GenericData.FirstOrDefault(d => d.DataKey == "Container").DataValue
            });

            if (mpeg4Data.Count() > 0)
            {
                reportBuilder.BuildReport(mpeg4Data.OrderBy(m => m.Movie).ThenBy(m => m.Year).ToList(), "mpeg4");
                mpeg4Data = null;
            }

            var aviData = mediaData.GetMovies(Classes.Enum.Container.avi).Select(m => new
            {
                Movie = m.Title,
                Year = m.Year,
                Codec = m.GenericData.FirstOrDefault(d => d.DataKey == "Video Codec").DataValue,
                Container = m.GenericData.FirstOrDefault(d => d.DataKey == "Container").DataValue
            });

            if (aviData.Count() > 0)
            {
                reportBuilder.BuildReport(aviData.OrderBy(m => m.Movie).ThenBy(m => m.Year).ToList(), "avi");
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
                DateAdded = m.Added,
                LastPlayed = m.LastPlayed
            });

            if (notWatchedMovies.Count() > 0)
            {
                reportBuilder.BuildReport(notWatchedMovies.OrderBy(m => m.Movie).ThenBy(m => m.Year).ToList(), "movies_stale");
                notWatchedMovies = null;
            }

            //TODO: TV Show Reports
        }
    }
}
