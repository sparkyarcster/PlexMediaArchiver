using System;

namespace PlexMediaArchiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new PlexAPI();
            var serverNames = api.ServerNames();

            //Stil unclear to me at this point if this data includes ALL home users, or simply the main Plex account.

            //var moviesNotViewed = api.GetMoviesNotViewed();

            //foreach(var movie in moviesNotViewed)
            //{
            //    var details = $"{movie.MetaData.Title} -> {(movie.LastViewed.HasValue ? movie.LastViewed.ToString() : "Never")}";
            //    Console.WriteLine(details);
            //}

            var movies = api.GetMoviesByTitle("Indiana");

            foreach (var movie in movies)
            {
                var details = $"{movie.MetaData.Title} -> {(movie.LastViewed.HasValue ? movie.LastViewed.ToString() : "Never")}";
                Console.WriteLine(details);
            }
        }
    }
}
