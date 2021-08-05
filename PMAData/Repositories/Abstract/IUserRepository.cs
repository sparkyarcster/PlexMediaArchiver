using System.Collections.Generic;

namespace PMAData.Repositories.Abstract
{
    public interface IUserRepository
    {
        void CreateOrUpdate(Model.User user);
    }
}
