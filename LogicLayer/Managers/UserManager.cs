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

            if (!user.EmailConfirmed)
                return null;

            return user;
        }


        public void CreateUser(User user)
        {
            if (userRepo.UsernameExists(user.Username))
                throw new UsernameAlreadyInUseException();

            if (userRepo.EmailExists(user.Gmail))
                throw new EmailAlreadyInUseException();

            if (!string.IsNullOrWhiteSpace(user.SteamId) &&
                userRepo.SteamIdExists(user.SteamId))
                throw new Exception("SteamID already in use");

            if (!user.Password.StartsWith("$2"))
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            user.EmailConfirmed = false;
            user.EmailToken = Guid.NewGuid().ToString();
            user.TokenCreatedAt = DateTime.Now;

            userRepo.CreateUser(user);
        }

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
                return;

            if (user.Id is not int userId)
                throw new InvalidOperationException("The user account is invalid.");

            userRepo.ConfirmEmail(userId);
        }

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

        public void DeleteUserAsAdmin(int userId, int actingAdminId)
        {
            var actingAdmin = userRepo.GetUserById(actingAdminId)
                ?? throw new InvalidOperationException("Administrator account was not found.");

            if (!actingAdmin.IsAdmin)
                throw new InvalidOperationException("Only administrators can delete user accounts.");

            var user = userRepo.GetUserById(userId)
                ?? throw new InvalidOperationException("User was not found.");

            if (user.Id == actingAdminId)
                throw new InvalidOperationException("You cannot delete your own account.");

            if (user.IsAdmin)
                throw new InvalidOperationException("Administrator accounts cannot be deleted here.");

            if (!string.IsNullOrWhiteSpace(user.SteamId) && user.SteamId != "0")
                throw new InvalidOperationException("Remove the player profile before deleting this account.");

            userRepo.DeleteUser(user);
        }

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
