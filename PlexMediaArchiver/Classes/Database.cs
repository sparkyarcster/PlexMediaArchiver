using PMAData.Database;
using System.Configuration;
using System.IO;

namespace PlexMediaArchiver.Classes
{
    public static class Database
    {
        public static DatabaseConfig DatabaseConfig;
        public static PMAData.Database.Abstract.IDatabaseBootstrap DatabaseBootstrap;
        public static PMAData.Repositories.Abstract.IMovieRepository MovieRepository;
        public static PMAData.Repositories.Abstract.ITVShowRepository TVShowRepository;
        public static PMAData.Repositories.Abstract.IUserRepository UserRepository;

        public static void Setup()
        {
            DatabaseConfig = new PMAData.Database.DatabaseConfig() { Name = ConfigurationManager.AppSettings["DatabaseName"] };
            DatabaseBootstrap = new PMAData.Database.DatabaseBootstrap(DatabaseConfig);

            DatabaseBootstrap.Setup();

            TVShowRepository = new PMAData.Repositories.TVShowRepository(DatabaseConfig);
            MovieRepository = new PMAData.Repositories.MovieRepository(DatabaseConfig);
            UserRepository = new PMAData.Repositories.UserRepository(DatabaseConfig);
        }

        public static void Reset()
        {
            var dbName = ConfigurationManager.AppSettings["DatabaseName"];

            if (string.IsNullOrEmpty(dbName))
            {
                return;
            }

            var currentPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            var dbFile = Path.Combine(currentPath, $"{dbName}.db");

            if (File.Exists(dbFile))
            {
                File.Delete(dbFile);
            }
        }
    }
}
