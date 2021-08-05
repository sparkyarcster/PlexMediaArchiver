using System.Configuration;
using System.Linq;

namespace PlexMediaArchiver
{
    class Program
    {
        static void Main(string[] args)
        {
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
                Classes.Database.Reset();
            }

            Classes.Database.Setup();

            var mediaData = new MediaData();

            if (resetDatabase)
            {
                mediaData.LoadData();
            }

            var reportBuilder = new ReportBuilder();

            reportBuilder.BuildReport(mediaData.GetMovies(Classes.Enum.MediaCodec.hevc).Select(m => new
            {
                Movie = m.Title,
                Codec = "hevc"
            }).ToList(), "hevc");

            reportBuilder.BuildReport(mediaData.GetMovies(Classes.Enum.MediaCodec.mpeg4).Select(m => new
            {
                Movie = m.Title,
                Codec = "mpeg4"
            }).ToList(), "mpeg4");
        }
    }
}
