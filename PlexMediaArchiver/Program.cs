using System;

namespace PlexMediaArchiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new PlexAPI();
            var serverNames = api.ServerNames();

            var movieLibrary = api.GetMovieLibrary();
            
            foreach(var movie in movieLibrary.MediaContainer.Metadata)
            {
                Console.WriteLine(movie.Title);
            }
        }
    }
}
