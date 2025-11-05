using LogicLayer;
using LogicLayer.Models;
using LogicLayer.IRepos;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace DataLayer.Repos
{
    public class TournamentRepo : ITournamentRepo
    {
        private readonly IConnection conn;

        public TournamentRepo(IConnection conn)
        {
            this.conn = conn;
        }

        // ✅ Load all tournaments
        public List<Tournament> GetAllTournaments()
        {
            var cmd = new MySqlCommand(
                "SELECT id, name, description, status FROM tournament",
                conn.GetInnerConn());

            var tournaments = new List<Tournament>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tournaments.Add(new Tournament(
                            reader.GetInt32("id"),
                            reader.GetString("name"),
                            reader.GetString("description"),
                            reader.GetString("status") == "Closed" // Convert enum to bool Closed
                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error loading tournaments.", ex);
            }

            return tournaments;
        }

        // ✅ Add new tournament
        public void AddTournament(Tournament tournament)
        {
            var cmd = new MySqlCommand(
                "INSERT INTO tournament (name, description, status) VALUES (@NAME, @DESCRIPTION, 'Open')",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@NAME", tournament.Name);
            cmd.Parameters.AddWithValue("@DESCRIPTION", tournament.Description);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error adding tournament.", ex);
            }
        }

        // ✅ Add application to tournament
        public void AddTournamentApp(Player player, Tournament tournament)
        {
            var cmd = new MySqlCommand(
                "INSERT INTO applications (tournamentId, playerId) VALUES(@tournamentId, @playerId)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@playerId", player.Id);
            cmd.Parameters.AddWithValue("@tournamentId", tournament.Id);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error adding player to tournament.", ex);
            }
        }

        // ✅ Remove tournament and dependencies
        public void RemoveTournament(Tournament tournament)
        {
            try
            {
                var deleteMatchesCmd = new MySqlCommand(
                    "DELETE FROM matches WHERE tournamentId = @TournamentId",
                    conn.GetInnerConn());
                deleteMatchesCmd.Parameters.AddWithValue("@TournamentId", tournament.Id);
                deleteMatchesCmd.ExecuteNonQuery();

                var deleteApplicationsCmd = new MySqlCommand(
                    "DELETE FROM applications WHERE tournamentId = @TournamentId",
                    conn.GetInnerConn());
                deleteApplicationsCmd.Parameters.AddWithValue("@TournamentId", tournament.Id);
                deleteApplicationsCmd.ExecuteNonQuery();

                var deleteTournamentCmd = new MySqlCommand(
                    "DELETE FROM tournament WHERE id = @ID",
                    conn.GetInnerConn());
                deleteTournamentCmd.Parameters.AddWithValue("@ID", tournament.Id);
                deleteTournamentCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error removing tournament.", ex);
            }
        }

        // ✅ Update tournament
        public void UpdateTournament(Tournament tournament)
        {
            var cmd = new MySqlCommand(
                "UPDATE tournament SET name = @NAME, description = @DESCRIPTION, status = @STATUS WHERE id = @ID",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@ID", tournament.Id);
            cmd.Parameters.AddWithValue("@NAME", tournament.Name);
            cmd.Parameters.AddWithValue("@DESCRIPTION", tournament.Description);
            cmd.Parameters.AddWithValue("@STATUS", tournament.Closed ? "Closed" : "Open");

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error updating tournament.", ex);
            }
        }

        // ✅ Get all players inside one tournament
        public List<Player> GetAllPlayersInTournament(int tournamentId)
        {
            var players = new List<Player>();

            var cmd = new MySqlCommand(@"
                SELECT user.*
                FROM user
                JOIN applications ON user.id = applications.playerid
                WHERE applications.tournamentId = @tournamentId;",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@tournamentId", tournamentId);

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var player = new Player(
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

                        players.Add(player);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error loading players for tournament.", ex);
            }

            return players;
        }
    }
}
