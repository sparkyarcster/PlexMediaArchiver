using System;

namespace PlexMediaArchiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var plexAPI = new PlexAPI();
            var plexServer = plexAPI.GetPreferredServer();

            if (plexServer != null)
            {
                var tautulliAPI = new TautulliAPI(plexServer);

                var movieLibrary = tautulliAPI.GetMovieLibrary();
                var movies = tautulliAPI.GetLibraryMediaInfoBySectionID(movieLibrary.SectionID, sectionType: movieLibrary.SectionType, length: movieLibrary.Count);

                Console.WriteLine(movies.Data.Count);

                var tvLibraryLibrary = tautulliAPI.GetTVLibrary();
                var tvshows = tautulliAPI.GetLibraryMediaInfoBySectionID(tvLibraryLibrary.SectionID, sectionType: tvLibraryLibrary.SectionType, length: tvLibraryLibrary.Count);

                Console.WriteLine(tvshows.Data.Count);
            }
            else
            {
                Console.WriteLine("COuld not locate preferred Plex server.");
            }
        }
    }
}
