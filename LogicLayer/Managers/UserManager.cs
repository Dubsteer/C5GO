using LogicLayer.Exceptions;
using LogicLayer.IRepos;
using LogicLayer.Models;
using System;
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

        // =====================================================
        // READ
        // =====================================================
        public List<User> GetAllUsers()
        {
            return userRepo.GetAllUsers();
        }

        public User? GetUserById(int id)
        {
            return userRepo.GetUserById(id);
        }

        public User? GetByUsername(string username)
        {
            return userRepo.GetUserByUsername(username);
        }


        // =====================================================
        // LOGIN
        // =====================================================
        public User? GetLoginUser(string username, string password)
        {
            var user = userRepo.GetAllUsers()
                .FirstOrDefault(u => u.Username == username);

            if (user == null)
                return null;

            bool passwordOk =
                user.Password == password ||
                (user.Password.StartsWith("$2") &&
                 BCrypt.Net.BCrypt.Verify(password, user.Password));

            if (!passwordOk)
                return null;

            // ❗❗❗ NEMA EXCEPTIONA
            if (!user.EmailConfirmed)
                return null;

            return user;
        }


        // =====================================================
        // CREATE (REGISTER)
        // =====================================================
        public void CreateUser(User user)
        {
            if (userRepo.UsernameExists(user.Username))
                throw new UsernameAlreadyInUseException();

            if (userRepo.EmailExists(user.Gmail))
                throw new EmailAlreadyInUseException();

            if (!string.IsNullOrWhiteSpace(user.SteamId) &&
                userRepo.SteamIdExists(user.SteamId))
                throw new Exception("SteamID already in use");

            // PASSWORD HASH
            if (!user.Password.StartsWith("$2"))
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // EMAIL VERIFICATION SETUP
            user.EmailConfirmed = false;
            user.EmailToken = Guid.NewGuid().ToString();
            user.TokenCreatedAt = DateTime.Now;

            userRepo.CreateUser(user);
        }

        // =====================================================
        // EMAIL VERIFICATION
        // =====================================================
        public User? GetUserByEmailToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            return userRepo.GetUserByEmailToken(token);
        }

        public void ConfirmEmail(string token)
        {
            var user = userRepo.GetUserByEmailToken(token);

            if (user == null)
                throw new Exception("Invalid or expired verification token.");

            if (user.EmailConfirmed)
                return; // already confirmed

            userRepo.ConfirmEmail(user.Id.Value);
        }

        // =====================================================
        // UPDATE / DELETE
        // =====================================================
        public void UpdateUser(User user)
        {
            var all = userRepo.GetAllUsers();

            if (all.Any(u => u.Username == user.Username && u.Id != user.Id))
                throw new Exception("Username already exists!");

            userRepo.UpdateUser(user);
        }

        public void DeleteUser(User user)
        {
            userRepo.DeleteUser(user);
        }

        // =====================================================
        // HELPERS
        // =====================================================
        public bool CheckIfUsernameExists(string username, int selfId)
        {
            return userRepo.CheckIfUsernameExists(username, selfId);
        }

        public List<User> SearchUser(string term)
        {
            return userRepo.SearchUser(term);
        }

        public bool SteamIdExists(string steamId)
        {
            return userRepo.SteamIdExists(steamId);
        }

        public bool IsAdmin(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            var user = userRepo.GetAllUsers()
                .FirstOrDefault(u => u.Username == username);

            return user != null && user.IsAdmin;
        }

    }
}
