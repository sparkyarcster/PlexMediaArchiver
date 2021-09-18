using PMAData.Model;
using PMAData.Repositories.Abstract;
using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace PMAData.Repositories
{
    public class MovieRepository : BaseRepository, IMovieRepository
    {
        public MovieRepository(Database.DatabaseConfig databaseConfig, NLog.Logger logger) : base(databaseConfig, logger) { }

        public void CreateOrUpdate(Movie movie)
        {
            DoCreateOrUpdate(movie);
        }

        private void DoCreateOrUpdate(Movie movie, int retry = 0)
        {
            try
            {
                using (var conn = databaseConfig.NewConnection())
                {
                    var dbMovie = conn.QueryFirstOrDefault<Movie>($"SELECT * FROM Movie WHERE ID = @MovieID", new { MovieID = movie.ID });

                    string sql = "";

                    if (dbMovie != null)
                    {
                        sql = "UPDATE Movie SET Title = @Title, Year = @Year, LastPlayed = @LastPlayed, Added = @Added, FileSize = @FileSize WHERE ID = @ID;";
                    }
                    else
                    {
                        sql = "INSERT INTO Movie (ID, Title, Year, LastPlayed, Added, FileSize) VALUES (@ID, @Title, @Year, @LastPlayed, @Added, @FileSize);";
                    }

                    if (!string.IsNullOrEmpty(sql))
                    {
                        conn.Execute(sql, movie);
                    }

                    conn.Execute($"DELETE FROM GenericData WHERE ID = @MediaID AND MediaType = @MediaType", new { MediaID = movie.ID, MediaType = "movie" });

                    if (movie.GenericData.Any())
                    {
                        foreach (var generic in movie.GenericData)
                        {
                            sql = "INSERT INTO GenericData (ID, MediaID, MediaType, DataKey, DataValue) VALUES (@ID, @MediaID, @MediaType, @DataKey, @DataValue)";

                            conn.Execute(sql, generic);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Error(ex, $"Error: {movie.Title}");

                if (ex.Message.Contains(" locked"))
                {
                    if (retry < 3)
                    {
                        logger.Info($"Retrying...");
                        DoCreateOrUpdate(movie, retry + 1);
                    }
                    else
                    {
                        logger.Error($"Fatal error: {movie.Title}");
                    }
                }
            }
        }

        public List<Movie> GetMovies()
        {
            if (!databaseConfig.HasConnection)
            {
                return new List<Movie>();
            }

            return databaseConfig.Connection.Query<Movie>($"SELECT * FROM Movie").ToList();
        }

        public List<Movie> GetMoviesByCodec(string codec)
        {
            return GetMoviesByGenericData("Video Codec", codec);
        }

        public List<Movie> GetMoviesByContainer(string container)
        {
            return GetMoviesByGenericData("Container", container);
        }

        private List<Movie> GetMoviesByGenericData(string dataKey, string dataValue)
        {
            if (!databaseConfig.HasConnection)
            {
                return new List<Movie>();
            }

            var movies = databaseConfig.Connection.Query<Movie>(@$"
SELECT Movie.*
  FROM Movie
 INNER JOIN GenericData
         ON Movie.ID = GenericData.MediaID
 WHERE GenericData.MediaType = 'movie'
   AND GenericData.DataKey = '{dataKey}'
   AND GenericData.DataValue LIKE '%{dataValue}%'").ToList();

            foreach (var movie in movies)
            {
                movie.GenericData = databaseConfig.Connection.Query<GenericData>("SELECT * FROM GenericData WHERE MediaID = @MediaID AND MediaType = 'movie'", new { MediaID = movie.ID }).ToList();
            }

            return movies;
        }

        public int GetCount()
        {
            if (!databaseConfig.HasConnection)
            {
                return 0;
            }

            try
            {
                return databaseConfig.Connection.Query<int>(@$"SELECT COUNT(1) FROM Movie").FirstOrDefault();
            }
            catch (System.Exception ex)
            {
                logger.Error(ex);
                return -1;
            }
        }
    }
}
