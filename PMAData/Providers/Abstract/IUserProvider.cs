using System;
using System.Collections.Generic;
using System.Text;

namespace PMAData.Providers.Abstract
{
    public interface IUserProvider
    {
        Model.User GetUser(int id);
        List<Model.User> GetUsers();
        void SaveUser(Model.User user);
    }
}
