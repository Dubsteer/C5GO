using LogicLayer.Models;
using System.Collections.Generic;

namespace LogicLayer.IRepos
{
    public interface IUserRepo
    {
        List<User> GetAllUsers();

        User? GetUserById(int id);

        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);

        bool CheckIfUsernameExists(string username, int selfId);

        List<User> SearchUser(string term);
    }
}
