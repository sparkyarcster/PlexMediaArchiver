using PMAData.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMAData.Providers
{
    public class TVShowProvider : BaseProvider, Abstract.ITVShowProvider
    {
        public TVShowProvider(Database.DatabaseConfig databaseConfig) : base(databaseConfig) { }

        public TVShow GetTVShow(int id)
        {
            if (!databaseConfig.HasConnection)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        public List<TVShow> GetTVShows()
        {
            if (!databaseConfig.HasConnection)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        public void SaveTVShow(TVShow tvshow)
        {
            if (!databaseConfig.HasConnection)
            {
                return;
            }

            throw new NotImplementedException();
        }
    }
}
