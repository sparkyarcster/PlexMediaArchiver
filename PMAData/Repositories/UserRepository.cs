using PMAData.Model;
using PMAData.Repositories.Abstract;
using Dapper;
using System.Linq;

namespace PMAData.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(Database.DatabaseConfig databaseConfig) : base(databaseConfig) { }

        public void CreateOrUpdate(User user)
        {
            DoCreateOrUpdate(user);
        }

        public void DoCreateOrUpdate(User user, int retry = 0)
        {
            try
            {
                using (var conn = databaseConfig.NewConnection())
                {
                    var dbUser = conn.QueryFirstOrDefault<User>($"SELECT * FROM User WHERE ID = @UserID", new { UserID = user.ID });

                    string sql = "";

                    if (dbUser != null)
                    {
                        sql = "UPDATE User SET UserName = @UserName, LastLogin = @LastLogin, LastActivity = @LastActivity WHERE ID = @ID;";
                    }
                    else
                    {
                        sql = "INSERT INTO User (ID, UserName, LastLogin, LastActivity) VALUES (@ID, @UserName, @LastLogin, @LastActivity);";
                    }

                    if (!string.IsNullOrEmpty(sql))
                    {
                        conn.Execute(sql, user);
                    }

                    conn.Execute($"DELETE FROM GenericData WHERE ID = @MediaID AND MediaType = @MediaType", new { MediaID = user.ID, MediaType = "user" });

                    if (user.GenericData.Any())
                    {
                        foreach (var generic in user.GenericData)
                        {
                            sql = "INSERT INTO GenericData (ID, MediaID, MediaType, DataKey, DataValue) VALUES (@ID, @MediaID, @MediaType, @DataKey, @DataValue)";

                            conn.Execute(sql, generic);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error: {user.UserName} -- {ex.Message}");

                if (ex.Message.Contains(" locked"))
                {
                    if (retry < 3)
                    {
                        DoCreateOrUpdate(user, retry + 1);
                    }
                }
            }
        }
    }
}
