using System;
using System.Collections.Generic;
using System.Text;

namespace PMAData.Providers.Abstract
{
    public interface ITVShowProvider
    {
        Model.TVShow GetTVShow(int id);
        List<Model.TVShow> GetTVShows();
        void SaveTVShow(Model.TVShow tvshow);
    }
}
