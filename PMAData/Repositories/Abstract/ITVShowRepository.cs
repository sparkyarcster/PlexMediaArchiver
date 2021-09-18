using System.Collections.Generic;

namespace PMAData.Repositories.Abstract
{
    public interface ITVShowRepository
    {
        void CreateOrUpdate(Model.TVShow tvshow);
        int GetCount();
        List<Model.TVShow> GetTVShows();
    }
}
