using System;
using System.Collections.Generic;
using System.Text;

namespace PMAData.Providers.Abstract
{
    public interface IMovieProvider
    {
        Model.Movie GetMovie(int id);
        List<Model.Movie> GetMovies();
        void SaveMovie(Model.Movie movie);
    }
}
