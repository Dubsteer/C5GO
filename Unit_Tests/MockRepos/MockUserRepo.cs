using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicLayer.IRepos;
using LogicLayer.Models;

namespace Unit_Tests.MockRepos
{
    public class MockUserRepo : IUserRepo
    {
        public List<User> Users = new();
        public MockUserRepo(List<User> users) 
        {
            Users = users;
        }

        public bool CheckIfUsernameExists(string username, int selfId)
        {
            throw new NotImplementedException();
        }

        public void CreateUser(User user)
        {
           Users.Add(user);
        }

        public void DeleteUser(User user)
        {
            foreach (var u in Users.ToArray())
            {
                if (u.Id == user.Id)
                {
                    Users.Remove(u);
                }
            }
        }

        public List<User> GetAllUsers()
        {
            return Users;
        }

        public List<User> SearchUser(string term)
        {
            term = term.ToLower();
            return Users.Where(user => user.Username.ToLower().Contains(term)).ToList();
        }

        public void UpdateUser(User user)
        {
            foreach(var u in Users.ToArray())
            {
                if( u.Id == user.Id)
                {
                    Users.Remove(u);
                }
            }
            Users.Add(user);
        }
    }
}
