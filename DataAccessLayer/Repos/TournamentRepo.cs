using LogicLayer;
using LogicLayer.Enums;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace DataLayer.Repos
{
    public class TournamentRepo : ITournamentRepo
    {
        private readonly IConnection conn;

        public TournamentRepo(IConnection conn)
        {
            this.conn = conn;
        }

        private void EnsureConnection()
        {
            if (conn.GetInnerConn().State != System.Data.ConnectionState.Open)
                conn.Open();
        }

        // ---------------------------
        // GET ALL TOURNAMENTS
        // ---------------------------
        public List<Tournament> GetAllTournaments()
        {
            EnsureConnection();

            var list = new List<Tournament>();
            var cmd = new MySqlCommand(
                "SELECT id, name, description, status_int FROM tournament",
                conn.GetInnerConn());

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Tournament(
                    reader.GetInt32("id"),
                    reader.GetString("name"),
                    reader.GetString("description"),
                    (Status)reader.GetInt32("status_int")
                ));
            }

            return list;
        }

        // ---------------------------
        // ADD TOURNAMENT
        // ---------------------------
        public void AddTournament(Tournament t)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "INSERT INTO tournament (name, description, status_int) VALUES (@n, @d, @s)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@n", t.Name);
            cmd.Parameters.AddWithValue("@d", t.Description);
            cmd.Parameters.AddWithValue("@s", (int)t.Status);

            cmd.ExecuteNonQuery();
        }

        // ---------------------------
        // UPDATE TOURNAMENT
        // ---------------------------
        public void UpdateTournament(Tournament t)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "UPDATE tournament SET name=@n, description=@d, status_int=@s WHERE id=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@n", t.Name);
            cmd.Parameters.AddWithValue("@d", t.Description);
            cmd.Parameters.AddWithValue("@s", (int)t.Status);
            cmd.Parameters.AddWithValue("@id", t.Id);

            cmd.ExecuteNonQuery();
        }

        // ---------------------------
        // DELETE TOURNAMENT (+ MATCHES & APPS)
        // ---------------------------
        public void RemoveTournament(Tournament t)
        {
            EnsureConnection();

            // delete matches
            var deleteMatches = new MySqlCommand(
                "DELETE FROM matches WHERE tournamentId=@id",
                conn.GetInnerConn());
            deleteMatches.Parameters.AddWithValue("@id", t.Id);
            deleteMatches.ExecuteNonQuery();

            // delete applications
            var deleteApps = new MySqlCommand(
                "DELETE FROM applications WHERE tournamentId=@id",
                conn.GetInnerConn());
            deleteApps.Parameters.AddWithValue("@id", t.Id);
            deleteApps.ExecuteNonQuery();

            // delete tournament
            var deleteTournament = new MySqlCommand(
                "DELETE FROM tournament WHERE id=@id",
                conn.GetInnerConn());
            deleteTournament.Parameters.AddWithValue("@id", t.Id);
            deleteTournament.ExecuteNonQuery();
        }

        // ---------------------------
        // ADD PLAYER TO TOURNAMENT
        // ---------------------------
        public void AddTournamentApp(Player player, Tournament tournament)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "INSERT INTO applications (tournamentId, playerId) VALUES(@tid, @pid)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@tid", tournament.Id);
            cmd.Parameters.AddWithValue("@pid", player.Id);

            cmd.ExecuteNonQuery();
        }

        // ---------------------------
        // REMOVE PLAYER FROM TOURNAMENT
        // ---------------------------
        public void RemovePlayerFromTournament(Player player, Tournament tournament)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "DELETE FROM applications WHERE playerId=@pid AND tournamentId=@tid",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@pid", player.Id);
            cmd.Parameters.AddWithValue("@tid", tournament.Id);

            cmd.ExecuteNonQuery();
        }

        // ---------------------------
        // GET PLAYERS IN TOURNAMENT
        // ---------------------------
        public List<Player> GetAllPlayersInTournament(int tournamentId)
        {
            EnsureConnection();

            var players = new List<Player>();
            var cmd = new MySqlCommand(@"
                SELECT user.id, first_name, last_name, age, username, email, password, steam_id, is_moderator 
                FROM user
                JOIN applications ON user.id = applications.playerId
                WHERE applications.tournamentId = @id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", tournamentId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                players.Add(new Player(
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

            return players;
        }
    }
}
