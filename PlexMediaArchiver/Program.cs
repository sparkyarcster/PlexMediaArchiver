using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace PlexMediaArchiver
{
    class Program
    {
        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        static void Main(string[] args)
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);

            Classes.AppLogger.Setup();

            try
            {
                Classes.Mapper.Configure();

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

                /*
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
                */

                var mpeg4Data = mediaData.GetMovies(Classes.Enum.MediaCodec.mpeg4).Select(m => new
                {
                    Movie = m.Title,
                    Year = m.Year,
                    MediaSize = m.FileSize,
                    Codec = m.GenericData.FirstOrDefault(d => d.DataKey == "Video Codec").DataValue,
                    Container = m.GenericData.FirstOrDefault(d => d.DataKey == "Container").DataValue,
                    Drive = string.Join(";", m.GenericData.Where(d => d.DataKey == "Drive" && d.DataValue != null).Select(d => d.DataValue)),
                    Current = m.IsCurrent,
                    Archived = m.IsArchived
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
                    Container = m.GenericData.FirstOrDefault(d => d.DataKey == "Container").DataValue,
                    Drive = string.Join(";", m.GenericData.Where(d => d.DataKey == "Drive" && d.DataValue != null).Select(d => d.DataValue)),
                    Current = m.IsCurrent,
                    Archived = m.IsArchived
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
                        LastPlayed = m.LastPlayed,
                        Drive = string.Join(";", m.GenericData.Where(d => d.DataKey == "Drive" && d.DataValue != null).Select(d => d.DataValue)),
                        Current = m.IsCurrent,
                        Archived = m.IsArchived
                    });

                if (notWatchedMovies.Count() > 0)
                {
                    reportsMade.Add(reportBuilder.BuildReport(notWatchedMovies.OrderBy(m => m.Movie).ThenBy(m => m.Year).ToList(), "movies_stale"));
                    notWatchedMovies = null;
                }

                var tvshowIgnoreDate = DateTime.Now.AddDays(Classes.Constants.IgnoreTVShowsLessThanDaysOld * -1);
                var tvshowCompareDate = DateTime.Now.AddDays(Classes.Constants.TVAge * -1);

                var notWatchedTVShows = mediaData.GetTVShows()
                    .Where(s => ((s.Added.HasValue && s.Added.Value < tvshowIgnoreDate) || (!s.Added.HasValue))
                        && (!s.LastPlayed.HasValue || s.LastPlayed.Value < tvshowCompareDate))
                    .Select(s => new
                    {
                        TVShow = s.Title,
                        Year = s.Year,
                        MediaSize = s.FileSize,
                        Episodes = s.GenericData.FirstOrDefault(d => d.DataKey == "Episodes")?.DataValue ?? "0",
                        DateAdded = s.Added,
                        LastPlayed = s.LastPlayed,
                        Drive = string.Join(";", s.GenericData.Where(d => d.DataKey == "Drive" && d.DataValue != null).Select(d => d.DataValue)),
                        Current = s.IsCurrent,
                        Archived = s.IsArchived
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
            catch (Exception ex)
            {
                Classes.AppLogger.log.Error(ex, "There was an uncaught error.");
            }
            finally
            {
                SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            }
        }
    }
}
