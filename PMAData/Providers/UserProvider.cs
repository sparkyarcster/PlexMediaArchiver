using PMAData.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMAData.Providers
{
    public class UserProvider : BaseProvider, Abstract. IUserProvider
    {
        public UserProvider(Database.DatabaseConfig databaseConfig) :base(databaseConfig) { }

        public User GetUser(int id)
        {
            if (!databaseConfig.HasConnection)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        public List<User> GetUsers()
        {
            if (!databaseConfig.HasConnection)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        public void SaveUser(User user)
        {
            if (!databaseConfig.HasConnection)
            {
                return;
            }

            throw new NotImplementedException();
        }
    }
}
