using LogicLayer;
using LogicLayer.Models;
using LogicLayer.IRepos;
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

        // ✅ Update Steam ID for player
        public void InitializeRole(Player player)
        {
            var cmd = new MySqlCommand("UPDATE user SET steam_id = @steam_id WHERE id = @id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@steam_id", player.Steamaccountid);
            cmd.Parameters.AddWithValue("@id", player.Id);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Failed to initialize player role.", ex);
            }
        }

        // ✅ Add player to tournament
        public void AddPlayerToTournament(Player player, Tournament tournament)
        {
            var cmd = new MySqlCommand("INSERT INTO applications (tournamentId, playerid) VALUES (@tournamentId, @playerId)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@tournamentId", tournament.Id);
            cmd.Parameters.AddWithValue("@playerId", player.Id);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Failed to add player to tournament.", ex);
            }
        }

        // ✅ Remove Steam ID (delete role)
        public void DeletePlayerRole(Player player)
        {
            var checkCmd = new MySqlCommand("SELECT steam_id FROM user WHERE id = @id", conn.GetInnerConn());
            checkCmd.Parameters.AddWithValue("@id", player.Id);

            string steam_id;
            try
            {
                var result = checkCmd.ExecuteScalar();
                steam_id = result?.ToString() ?? "0";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error while checking player role.", ex);
            }

            if (steam_id == "0")
            {
                throw new Exception("The player doesn't have a role to delete.");
            }

            var cmd = new MySqlCommand("UPDATE user SET steam_id = '0' WHERE id = @id", conn.GetInnerConn());
            cmd.Parameters.AddWithValue("@id", player.Id);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error removing player role.", ex);
            }
        }

        // ✅ Get all players
        public List<Player> GetAllPlayers()
        {
            var cmd = new MySqlCommand("SELECT * FROM user", conn.GetInnerConn());

            var players = new List<Player>();
            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        players.Add(new Player(
                            reader.GetInt32("id"),
                            reader.GetString("first_name"),
                            reader.GetString("last_name"),
                            reader.IsDBNull(reader.GetOrdinal("age")) ? 0 : reader.GetInt32("age"),
                            reader.GetString("username"),
                            reader.GetString("email"),
                            reader.GetString("password"),
                            reader.GetString("steam_id"),
                            reader.GetBoolean("is_moderator") // ✅ fixed
                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error loading players.", ex);
            }

            return players;
        }

        // ✅ Get one player by user
        public Player GetPlayer(User user)
        {
            var cmd = new MySqlCommand("SELECT * FROM user WHERE id = @id", conn.GetInnerConn());
            cmd.Parameters.AddWithValue("@id", user.Id);

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Player(
                            reader.GetInt32("id"),
                            reader.GetString("first_name"),
                            reader.GetString("last_name"),
                            reader.IsDBNull(reader.GetOrdinal("age")) ? 0 : reader.GetInt32("age"),
                            reader.GetString("username"),
                            reader.GetString("email"),
                            reader.GetString("password"),
                            reader.GetString("steam_id"),
                            reader.GetBoolean("is_moderator") // ✅ fixed
                        );
                    }
                    else
                    {
                        throw new Exception("Player not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error loading player.", ex);
            }
        }
    }
}
