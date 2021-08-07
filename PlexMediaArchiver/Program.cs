using System.Configuration;
using System.Linq;

namespace PlexMediaArchiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Classes.AppLogger.Setup();

            bool resetDatabase;
            var configReset = ConfigurationManager.AppSettings["RefreshDatabase"];

            if (!string.IsNullOrEmpty(configReset))
            {
                if (!bool.TryParse(configReset, out resetDatabase))
                {
                    resetDatabase = false;
                }
            }
            else
            {
                resetDatabase = false;
            }

            if (resetDatabase)
            {
                System.Console.WriteLine("Are you sure you want to reset the database? Y/N");

                var confirm = System.Console.ReadKey();

                System.Console.WriteLine("");

                if (confirm.Key == System.ConsoleKey.Y)
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
            }

            //TODO: Not watched
            //TODO: TV Show Reports
        }
    }
}
