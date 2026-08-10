using LogicLayer.Models;

namespace LogicLayer.Models
{
    public class Player : User
    {
        public Player(User u)
            : base(u.Id, u.Firstname, u.Lastname, u.Age, u.Username, u.Gmail, u.Password, u.IsAdmin, u.SteamId)
        {
        }

        public Player(int id, string firstname, string lastname, int age,
                      string username, string gmail, string password,
                      string steamId, bool isAdmin)
            : base(id, firstname, lastname, age, username, gmail, password, isAdmin, steamId)
        {
        }
    }
}
