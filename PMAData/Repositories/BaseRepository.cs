using PMAData.Database;

namespace PMAData.Repositories
{
    public class BaseRepository
    {
        protected readonly DatabaseConfig databaseConfig;

        public BaseRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }
    }
}
