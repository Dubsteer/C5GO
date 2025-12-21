using LogicLayer.Models;
using System.Collections.Generic;

namespace LogicLayer.IRepos
{
    public interface IUserRepo
    {
        // =========================
        // READ
        // =========================
        List<User> GetAllUsers();
        User? GetUserById(int id);
        User? GetUserByEmailToken(string token);

        // =========================
        // CREATE / UPDATE / DELETE
        // =========================
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);

        // =========================
        // EMAIL VERIFICATION
        // =========================
        void ConfirmEmail(int userId);

        // =========================
        // EXISTENCE CHECKS
        // =========================
        bool UsernameExists(string username);
        bool EmailExists(string email);
        bool SteamIdExists(string steamId);
        bool CheckIfUsernameExists(string username, int selfId);

        // =========================
        // SEARCH
        // =========================
        List<User> SearchUser(string term);
    }
}
