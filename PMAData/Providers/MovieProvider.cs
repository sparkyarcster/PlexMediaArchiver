using PMAData.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMAData.Providers
{
    public class MovieProvider : BaseProvider, Abstract.IMovieProvider
    {
        public MovieProvider(Database.DatabaseConfig databaseConfig) : base(databaseConfig) { }

        public Movie GetMovie(int id)
        {
            if (!databaseConfig.HasConnection)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        public List<Movie> GetMovies()
        {
            if (!databaseConfig.HasConnection)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        public void SaveMovie(Movie movie)
        {
            if (!databaseConfig.HasConnection)
            {
                return;
            }

            throw new NotImplementedException();
        }
    }
}
