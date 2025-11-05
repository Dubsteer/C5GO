using LogicLayer.Models;

namespace LogicLayer.IRepos
{
    public interface IUserRepo
    {
        public void CreateUser(User user);

        public List<User> GetAllUsers();

        public void UpdateUser(User user);

        public void DeleteUser(User user);

        public bool CheckIfUsernameExists(string username, int selfId);

        public List<User> SearchUser(string term);
    }
}
