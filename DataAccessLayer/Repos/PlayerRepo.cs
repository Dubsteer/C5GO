using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System.Diagnostics;

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
                list.Add(new Player(
                    reader.GetInt32("id"),
                    reader.GetString("first_name"),
                    reader.GetString("last_name"),
                    reader.GetInt32("age"),
                    reader.GetString("username"),
                    reader.GetString("email"),
                    reader.GetString("password"),
                    reader.GetString("steam_id"),
                    reader.GetBoolean("is_moderator")
                ));
            }
            return list;
        }

        public Player GetPlayer(User user)
        {
            var cmd = new MySqlCommand("SELECT * FROM user WHERE id=@id", conn.GetInnerConn());
            cmd.Parameters.AddWithValue("@id", user.Id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string steamId = reader.GetString("steam_id");

                if (steamId == "0") return null; // not a player

                return new Player(
                    reader.GetInt32("id"),
                    reader.GetString("first_name"),
                    reader.GetString("last_name"),
                    reader.GetInt32("age"),
                    reader.GetString("username"),
                    reader.GetString("email"),
                    reader.GetString("password"),
                    steamId,
                    reader.GetBoolean("is_moderator")
                );
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
