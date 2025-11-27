using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace DataLayer.Repos
{
    public class PlayerRepo : IPlayerRepo
    {
        private readonly IConnection conn;

        public PlayerRepo(IConnection conn)
        {
            this.conn = conn;
        }

        public void InitializeRole(Player player)
        {
            var cmd = new MySqlCommand(
                "UPDATE user SET steam_id = @steam_id WHERE id = @id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@steam_id", player.SteamId);
            cmd.Parameters.AddWithValue("@id", player.Id);

            cmd.ExecuteNonQuery();
        }

        public List<Player> GetAllPlayers()
        {
            var cmd = new MySqlCommand("SELECT * FROM user WHERE steam_id != '0'", conn.GetInnerConn());
            var list = new List<Player>();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var u = new User(
                    reader.GetInt32("id"),
                    reader.GetString("first_name"),
                    reader.GetString("last_name"),
                    reader.GetInt32("age"),
                    reader.GetString("username"),
                    reader.GetString("email"),
                    reader.GetString("password"),
                    reader.GetBoolean("is_moderator"),
                    reader.GetString("steam_id")
                );

                list.Add(new Player(u));
            }

            return list;
        }

        public Player GetPlayer(User user)
        {
            var cmd = new MySqlCommand("SELECT * FROM user WHERE id=@id", conn.GetInnerConn());
            cmd.Parameters.AddWithValue("@id", user.Id);

            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                if (r.GetString("steam_id") == "0")
                    return null;

                var u = new User(
                    r.GetInt32("id"),
                    r.GetString("first_name"),
                    r.GetString("last_name"),
                    r.GetInt32("age"),
                    r.GetString("username"),
                    r.GetString("email"),
                    r.GetString("password"),
                    r.GetBoolean("is_moderator"),
                    r.GetString("steam_id")
                );

                return new Player(u);
            }

            return null;
        }

        public void AddPlayerToTournament(Player player, Tournament tournament)
        {
            var cmd = new MySqlCommand(
                "INSERT INTO applications (tournamentId, playerid) VALUES (@tid, @pid)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@tid", tournament.Id);
            cmd.Parameters.AddWithValue("@pid", player.Id);
            cmd.ExecuteNonQuery();
        }

        public void DeletePlayerRole(Player player)
        {
            var cmd = new MySqlCommand("UPDATE user SET steam_id = '0' WHERE id=@id", conn.GetInnerConn());
            cmd.Parameters.AddWithValue("@id", player.Id);
            cmd.ExecuteNonQuery();
        }
    }
}
