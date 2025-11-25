using LogicLayer.IRepos;
using LogicLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace LogicLayer.Managers
{
    public class UserManager
    {
        private readonly IUserRepo userRepo;

        public UserManager(IUserRepo userRepo)
        {
            this.userRepo = userRepo;
        }

        public List<User> GetAllUsers()
        {
            return userRepo.GetAllUsers();
        }

        // FIX — REAL DB CALL
        public User? GetUserById(int id)
        {
            return userRepo.GetUserById(id);
        }

        public User? GetLoginUser(string username, string password)
        {
            var user = userRepo.GetAllUsers()
                .FirstOrDefault(u => u.Username == username);

            if (user == null)
                return null;

            if (user.Password == password)
                return user;

            if (user.Password.StartsWith("$2"))
            {
                if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                    return user;
            }

            return null;
        }

        public void UpdateUser(User user)
        {
            var all = userRepo.GetAllUsers();

            if (all.Any(u => u.Username == user.Username && u.Id != user.Id))
                throw new System.Exception("Username already exists!");

            userRepo.UpdateUser(user);
        }

        public void DeleteUser(User user)
        {
            userRepo.DeleteUser(user);
        }

        public void CreateUser(User user)
        {
            if (!user.Password.StartsWith("$2"))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            userRepo.CreateUser(user);
        }

        public bool CheckIfUsernameExists(string username, int selfId)
        {
            return userRepo.CheckIfUsernameExists(username, selfId);
        }

        public List<User> SearchUser(string term)
        {
            return userRepo.SearchUser(term);
        }
    }
}
