using PMAData.Database;

namespace PMAData.Providers
{
    public class BaseProvider
    {
        protected readonly DatabaseConfig databaseConfig;

        public BaseProvider(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }
    }
}
