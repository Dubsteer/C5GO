using LogicLayer.IRepos;
using LogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using BCrypt.Net;

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

        public User? GetUserById(int id)
        {
            return userRepo.GetAllUsers().FirstOrDefault(u => u.Id == id);
        }

        public User? GetLoginUser(string username, string password)
        {
            var user = userRepo.GetAllUsers().FirstOrDefault(u => u.Username == username);

            if (user == null)
                return null;

            // ✅ Allow both plain-text and hashed passwords
            try
            {
                // 1️⃣ If it's plain text (used in your local DB), just compare directly
                if (user.Password == password)
                    return user;

                // 2️⃣ If it's hashed (BCrypt), verify it
                if (user.Password.StartsWith("$2"))
                {
                    if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                        return user;
                }

                // 3️⃣ If neither matches, login fails
                return null;
            }
            catch (Exception ex)
            {
                // Handle any weird edge case (e.g., invalid salt)
                Console.WriteLine($"BCrypt check failed: {ex.Message}");
                return null;
            }
        }

        public void UpdateUser(User user)
        {
            var all = userRepo.GetAllUsers();

            // Check username uniqueness
            if (all.Any(u => u.Username == user.Username && u.Id != user.Id))
                throw new Exception("Username already exists!");

            userRepo.UpdateUser(user);
        }


        public void DeleteUser(User user)
        {
            userRepo.DeleteUser(user);
        }

        public void CreateUser(User user)
        {
            // ✅ Automatically hash password when adding a new user
            // (optional, can be removed if you want to keep plain text)
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
