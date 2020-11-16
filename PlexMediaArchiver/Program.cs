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

<<<<<<< HEAD
            var moviesNotViewed = api.GetMoviesNotViewed();

            foreach(var movie in moviesNotViewed)
=======
                var movieLibrary = tautulliAPI.GetMovieLibrary();
                var movies = tautulliAPI.GetLibraryMediaInfoBySectionID(movieLibrary.SectionID, sectionType: movieLibrary.SectionType, length: movieLibrary.Count);

                Console.WriteLine(movies.Data.Count);

                var tvLibraryLibrary = tautulliAPI.GetTVLibrary();
                var tvshows = tautulliAPI.GetLibraryMediaInfoBySectionID(tvLibraryLibrary.SectionID, sectionType: tvLibraryLibrary.SectionType, length: tvLibraryLibrary.Count);

                Console.WriteLine(tvshows.Data.Count);
            }
            else
>>>>>>> c5be55ddd8c9bba6f11f66ff68e00af49bb0d306
            {
                Console.WriteLine("COuld not locate preferred Plex server.");
            }

            //var movies = api.GetMoviesByTitle("Indiana");

            //foreach (var movie in movies)
            //{
              //  var details = $"{movie.MetaData.Title} -> {(movie.LastViewed.HasValue ? movie.LastViewed.ToString() : "Never")}";
                //Console.WriteLine(details);
            }
        }
    }
}
