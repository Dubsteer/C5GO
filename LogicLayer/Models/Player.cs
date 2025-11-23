namespace LogicLayer.Models
{
    public class Player : User
    {
        public Player(int id, string firstname, string lastname, int age, string username, string gmail,
                      string password, string steamId, bool isAdmin)
            : base(id, firstname, lastname, age, username, gmail, password, isAdmin, steamId)
        {
        }
    }
}
