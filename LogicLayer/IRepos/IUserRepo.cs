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

        // EXISTENCE CHECKS (NEW)
        bool UsernameExists(string username);
        bool EmailExists(string email);
        bool SteamIdExists(string steamId);

        bool CheckIfUsernameExists(string username, int selfId);

        List<User> SearchUser(string term);
    }
}
