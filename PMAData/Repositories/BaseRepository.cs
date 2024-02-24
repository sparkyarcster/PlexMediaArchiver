using PMAData.Database;

namespace PMAData.Repositories
{
    public class BaseRepository
    {
        protected readonly DatabaseConfig databaseConfig;
        protected readonly NLog.Logger logger;

        public BaseRepository(DatabaseConfig databaseConfig, NLog.Logger logger)
        {
            this.databaseConfig = databaseConfig;
            this.logger = logger;
        }
    }
}
