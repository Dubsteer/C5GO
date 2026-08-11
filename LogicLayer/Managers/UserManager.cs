using LogicLayer.Exceptions;
using LogicLayer.IRepos;
using LogicLayer.Models;
using LogicLayer.Services;
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

        public User? GetUserByEmail(string email)
        {
            return string.IsNullOrWhiteSpace(email)
                ? null
                : userRepo.GetUserByEmail(email.Trim());
        }

        public User? GetByUsername(string username)
        {
            return userRepo.GetUserByUsername(username);
        }


        public User? GetLoginUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
                return null;

            var user = userRepo.GetUserByUsername(username.Trim());

            if (user == null)
                return null;

            var passwordOk = false;
            var isBcryptHash = user.Password.StartsWith("$2", StringComparison.Ordinal);

            if (isBcryptHash)
            {
                try
                {
                    passwordOk = BCrypt.Net.BCrypt.Verify(password, user.Password);
                }
                catch (BCrypt.Net.SaltParseException)
                {
                    return null;
                }
            }
            else if (string.Equals(user.Password, password, StringComparison.Ordinal))
            {
                passwordOk = true;

                if (user.Id is int userId)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                    userRepo.UpdatePassword(userId, user.Password);
                }
            }

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
            user.EmailToken = Guid.NewGuid().ToString("N");
            user.TokenCreatedAt = DateTime.UtcNow;

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

            if (!user.TokenCreatedAt.HasValue ||
                DateTime.UtcNow - user.TokenCreatedAt.Value > TimeSpan.FromHours(24))
            {
                throw new Exception("Invalid or expired verification token.");
            }

            if (user.Id is not int userId)
                throw new InvalidOperationException("The user account is invalid.");

            userRepo.ConfirmEmail(userId);
        }

        public void UpdateUser(User user)
        {
            if (user.Id is not int userId)
                throw new InvalidOperationException("The user account is invalid.");

            var all = userRepo.GetAllUsers();

            if (all.Any(u =>
                    u.Id != userId &&
                    string.Equals(u.Username, user.Username, StringComparison.OrdinalIgnoreCase)))
            {
                throw new UsernameAlreadyInUseException();
            }

            if (all.Any(u =>
                    u.Id != userId &&
                    string.Equals(u.Gmail, user.Gmail, StringComparison.OrdinalIgnoreCase)))
            {
                throw new EmailAlreadyInUseException();
            }

            if (!SteamIdParser.TryNormalize(user.SteamId, out var normalizedSteamId))
                throw new InvalidSteamIdException();

            if (user.IsAdmin && normalizedSteamId != null)
                throw new InvalidOperationException("Administrator accounts cannot have a player profile.");

            if (normalizedSteamId != null && all.Any(u =>
                    u.Id != userId &&
                    SteamIdParser.TryNormalize(u.SteamId, out var existingSteamId) &&
                    string.Equals(existingSteamId, normalizedSteamId, StringComparison.Ordinal)))
            {
                throw new SteamIdAlreadyInUseException();
            }

            user.SteamId = normalizedSteamId;
            userRepo.UpdateUser(user);
        }

        public bool ResetPassword(int userId, string currentPasswordHash, string newPassword)
        {
            if (newPassword.Length is < 8 or > 72)
                throw new ArgumentException("Password must be between 8 and 72 characters.", nameof(newPassword));

            var user = userRepo.GetUserById(userId);
            if (user == null || !user.EmailConfirmed)
                return false;

            if (!string.Equals(user.Password, currentPasswordHash, StringComparison.Ordinal))
                return false;

            userRepo.UpdatePassword(userId, BCrypt.Net.BCrypt.HashPassword(newPassword));
            return true;
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
