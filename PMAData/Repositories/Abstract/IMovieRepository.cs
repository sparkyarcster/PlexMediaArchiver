using System.Collections.Generic;

namespace PMAData.Repositories.Abstract
{
    public interface IMovieRepository
    {
        void CreateOrUpdate(Model.Movie movie);
        List<Model.Movie> GetMovies();
        List<Model.Movie> GetMoviesByCodec(string codec);
        List<Model.Movie> GetMoviesByContainer(string codec);
        int GetCount();
    }
}
