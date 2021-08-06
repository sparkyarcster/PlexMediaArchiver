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

            reportBuilder.BuildReport(mediaData.GetMovies(Classes.Enum.MediaCodec.hevc).Select(m => new
            {
                Movie = m.Title,
                Year = m.Year,
                Codec = m.GenericData.FirstOrDefault(d => d.DataKey == "Video Codec").DataValue
            }).OrderBy(m => m.Movie).ThenBy(m => m.Year).ToList(), "hevc");

            reportBuilder.BuildReport(mediaData.GetMovies(Classes.Enum.MediaCodec.mpeg4).Select(m => new
            {
                Movie = m.Title,
                Year = m.Year,
                Codec = m.GenericData.FirstOrDefault(d => d.DataKey == "Video Codec").DataValue
            }).OrderBy(m => m.Movie).ThenBy(m => m.Year).ToList(), "mpeg4");
        }
    }
}
