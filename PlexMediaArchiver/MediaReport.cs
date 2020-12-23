using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace PlexMediaArchiver
{
    public class MediaReport
    {

        public Helpers.MediaItems RunReports()
        {
            var rtn = new Helpers.MediaItems();
            var plexAPI = new PlexAPI();
            var plexServer = plexAPI.GetPreferredServer();
            var outputData = new System.Text.StringBuilder();
            var runPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            var reportPath = System.IO.Path.Combine(runPath, "reports");
            var filePath = System.IO.Path.Combine(reportPath, $"report_{DateTime.Now.ToString("yyyyMMdd_hhmmss")}.csv");
            var movieAge = int.Parse(ConfigurationManager.AppSettings["MovieAge"] ?? "365");
            var tvAge = int.Parse(ConfigurationManager.AppSettings["TVAge"] ?? "730");

            if (!System.IO.Directory.Exists(reportPath))
            {
                System.IO.Directory.CreateDirectory(reportPath);
            }

            if (plexServer != null)
            {
                var tautulliAPI = new TautulliAPI(plexServer);

                var movieLibrary = tautulliAPI.GetMovieLibrary();
                var movies = tautulliAPI.GetLibraryMediaInfoBySectionID(movieLibrary.SectionID, sectionType: movieLibrary.SectionType, length: movieLibrary.Count);
                var movieCompareDate = DateTime.Now.AddDays(movieAge * -1);
                
                outputData.AppendLine("MOVIES");

                foreach (var movie in movies.Data)
                {
                    var lastViewed = ConvertLastViewed(movie.last_played ?? 0);

                    if (!lastViewed.HasValue || lastViewed.Value < movieCompareDate)
                    {
                        if (movie.title.Contains("\"") || movie.title.Contains(","))
                        {
                            outputData.Append($"\"{movie.title.Replace("\"", "\\\"")}\"");
                        }
                        else
                        {
                            outputData.Append($"{movie.title}");
                        }

                        outputData.Append($",{movie.year}");
                        outputData.Append($",{(lastViewed.HasValue ? lastViewed.Value.ToString("yyyy-MM-dd") : "Never")}");
                        outputData.Append(Environment.NewLine);

                        rtn.Movies.Add(new Helpers.MediaItem()
                        {
                            Title = movie.title,
                            Year = movie.year,
                            LastPlayed = lastViewed
                        });
                    }
                }

                outputData.AppendLine("TV SHOWS");

                var tvLibraryLibrary = tautulliAPI.GetTVLibrary();
                var tvshows = tautulliAPI.GetLibraryMediaInfoBySectionID(tvLibraryLibrary.SectionID, sectionType: tvLibraryLibrary.SectionType, length: tvLibraryLibrary.Count);
                var tvCompareDate = DateTime.Now.AddDays(tvAge * -1);

                foreach (var tvshow in tvshows.Data)
                {
                    var lastViewed = ConvertLastViewed(tvshow.last_played ?? 0);

                    if (!lastViewed.HasValue || lastViewed.Value < tvCompareDate)
                    {
                        if (tvshow.title.Contains("\"") || tvshow.title.Contains(","))
                        {
                            outputData.Append($"\"{tvshow.title.Replace("\"", "\\\"")}\"");
                        }
                        else
                        {
                            outputData.Append($"{tvshow.title}");
                        }

                        outputData.Append($",{tvshow.year}");
                        outputData.Append($",{(lastViewed.HasValue ? lastViewed.Value.ToString("yyyy-MM-dd") : "Never")}");
                        outputData.Append(Environment.NewLine);

                        rtn.TVShows.Add(new Helpers.MediaItem()
                        {
                            Title = tvshow.title,
                            Year = tvshow.year,
                            LastPlayed = lastViewed
                        });
                    }
                }
            }
            else
            {
                Console.WriteLine("Could not locate preferred Plex server.");
            }

            System.IO.File.WriteAllText(filePath, outputData.ToString());

            Console.WriteLine($"File saved to '{filePath}'.");

            return rtn;
        }

        private string ToLiteral(string input)
        {
            using (var writer = new System.IO.StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
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
