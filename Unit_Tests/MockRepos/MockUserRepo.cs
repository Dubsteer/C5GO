using LogicLayer.IRepos;
using LogicLayer.Models;

namespace Unit_Tests.MockRepos
{
    public class MockUserRepo : IUserRepo
    {
        public List<User> Users { get; }

        public MockUserRepo(List<User> users)
        {
            Users = users;
        }

        public List<User> GetAllUsers() => Users;

        public User? GetUserById(int id) => Users.FirstOrDefault(u => u.Id == id);

        public User? GetUserByEmailToken(string token) =>
            Users.FirstOrDefault(u => u.EmailToken == token);

        public User? GetUserByUsername(string username) =>
            Users.FirstOrDefault(u =>
                string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));

        public void CreateUser(User user) => Users.Add(user);

        public void UpdateUser(User user)
        {
            Users.RemoveAll(u => u.Id == user.Id);
            Users.Add(user);
        }

        public void DeleteUser(User user) => Users.RemoveAll(u => u.Id == user.Id);

        public void ConfirmEmail(int userId)
        {
            var user = GetUserById(userId);
            if (user == null)
                return;

            user.EmailConfirmed = true;
            user.EmailToken = null!;
            user.TokenCreatedAt = null;
        }

        public bool UsernameExists(string username) =>
            Users.Any(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));

        public bool EmailExists(string email) =>
            Users.Any(u => string.Equals(u.Gmail, email, StringComparison.OrdinalIgnoreCase));

        public bool SteamIdExists(string steamId) =>
            Users.Any(u => !string.IsNullOrWhiteSpace(u.SteamId) && u.SteamId == steamId);

        public bool CheckIfUsernameExists(string username, int selfId) =>
            Users.Any(u =>
                u.Id != selfId &&
                string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));

        public List<User> SearchUser(string term) =>
            Users.Where(user =>
                user.Username.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                user.Gmail.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}
