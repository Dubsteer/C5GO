using LogicLayer;
using LogicLayer.Enums;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System;
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

        // ===========================================================
        // GET ALL
        // ===========================================================
        public List<Tournament> GetAllTournaments()
        {
            EnsureConnection();

            var list = new List<Tournament>();

            var cmd = new MySqlCommand(
                @"SELECT id, name, description, status_int, is_team, team_size_required 
                  FROM tournament",
                conn.GetInnerConn());

            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                var t = new Tournament
                {
                    Id = r.GetInt32("id"),
                    Name = r.GetString("name"),
                    Description = r.GetString("description"),
                    Status = (Status)r.GetInt32("status_int"),
                    IsTeamTournament = r.GetBoolean("is_team"),
                    TeamSizeRequired = r.GetInt32("team_size_required")
                };

                list.Add(t);
            }
            r.Close(); // CLOSE READER so new queries can run!!

            // --- Load counts for each tournament ---
            foreach (var t in list)
            {
                t.PlayersCount = GetPlayersCount(t.Id);
                t.TeamsCount = GetTeamsCount(t.Id);
                t.MatchesCount = GetMatchesCount(t.Id);
                t.CanLeave = false;
            }

            return list;
        }

        // ===========================================================
        // GET BY ID
        // ===========================================================
        public Tournament GetTournamentById(int id)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                @"SELECT id, name, description, status_int, is_team, team_size_required
                  FROM tournament WHERE id=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            var t = new Tournament
            {
                Id = r.GetInt32("id"),
                Name = r.GetString("name"),
                Description = r.GetString("description"),
                Status = (Status)r.GetInt32("status_int"),
                IsTeamTournament = r.GetBoolean("is_team"),
                TeamSizeRequired = r.GetInt32("team_size_required")
            };
            r.Close();

            // Load counts
            t.PlayersCount = GetPlayersCount(t.Id);
            t.TeamsCount = GetTeamsCount(t.Id);
            t.MatchesCount = GetMatchesCount(t.Id);
            t.CanLeave = false;

            return t;
        }

        // ===========================================================
        // ADD / UPDATE / DELETE
        // ===========================================================
        public void AddTournament(Tournament t)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                @"INSERT INTO tournament 
                  (name, description, status_int, is_team, team_size_required)
                  VALUES (@n, @d, @s, @team, @size)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@n", t.Name);
            cmd.Parameters.AddWithValue("@d", t.Description);
            cmd.Parameters.AddWithValue("@s", (int)t.Status);
            cmd.Parameters.AddWithValue("@team", t.IsTeamTournament);
            cmd.Parameters.AddWithValue("@size", t.TeamSizeRequired);

            cmd.ExecuteNonQuery();
        }

        public void UpdateTournament(Tournament t)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                @"UPDATE tournament 
                  SET name=@n, description=@d, status_int=@s, is_team=@team, team_size_required=@size
                  WHERE id=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", t.Id);
            cmd.Parameters.AddWithValue("@n", t.Name);
            cmd.Parameters.AddWithValue("@d", t.Description);
            cmd.Parameters.AddWithValue("@s", (int)t.Status);
            cmd.Parameters.AddWithValue("@team", t.IsTeamTournament);
            cmd.Parameters.AddWithValue("@size", t.TeamSizeRequired);

            cmd.ExecuteNonQuery();
        }

        public void RemoveTournament(Tournament t)
        {
            EnsureConnection();

            // Remove solo matches
            new MySqlCommand("DELETE FROM matches WHERE tournamentId=@id", conn.GetInnerConn())
            { Parameters = { new MySqlParameter("@id", t.Id) } }.ExecuteNonQuery();

            // Remove old match table
            new MySqlCommand("DELETE FROM `match` WHERE tournamentId=@id", conn.GetInnerConn())
            { Parameters = { new MySqlParameter("@id", t.Id) } }.ExecuteNonQuery();

            // Remove solo applications
            new MySqlCommand("DELETE FROM applications WHERE tournamentId=@id", conn.GetInnerConn())
            { Parameters = { new MySqlParameter("@id", t.Id) } }.ExecuteNonQuery();

            // Remove team applications
            new MySqlCommand("DELETE FROM team_applications WHERE tournamentId=@id", conn.GetInnerConn())
            { Parameters = { new MySqlParameter("@id", t.Id) } }.ExecuteNonQuery();

            // Remove tournament itself
            new MySqlCommand("DELETE FROM tournament WHERE id=@id", conn.GetInnerConn())
            { Parameters = { new MySqlParameter("@id", t.Id) } }.ExecuteNonQuery();
        }

        // ===========================================================
        // COUNTS
        // ===========================================================
        private int GetPlayersCount(int tournamentId)
        {
            var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM applications WHERE tournamentId=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", tournamentId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private int GetTeamsCount(int tournamentId)
        {
            var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM team_applications WHERE tournamentId=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", tournamentId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private int GetMatchesCount(int tournamentId)
        {
            var cmd = new MySqlCommand(
                @"SELECT 
                      (SELECT COUNT(*) FROM matches WHERE tournamentId=@id) +
                      (SELECT COUNT(*) FROM team_matches WHERE tournamentId=@id)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", tournamentId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // ===========================================================
        // SOLO
        // ===========================================================
        public void AddTournamentApp(Player p, Tournament t)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "INSERT INTO applications (tournamentId, playerId) VALUES (@t, @p)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@t", t.Id);
            cmd.Parameters.AddWithValue("@p", p.Id);

            cmd.ExecuteNonQuery();
        }

        public void RemovePlayerFromTournament(Player p, Tournament t)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "DELETE FROM applications WHERE tournamentId=@t AND playerId=@p",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@t", t.Id);
            cmd.Parameters.AddWithValue("@p", p.Id);

            cmd.ExecuteNonQuery();
        }

        public List<Player> GetAllPlayersInTournament(int id)
        {
            EnsureConnection();

            var list = new List<Player>();

            var cmd = new MySqlCommand(
                @"SELECT user.id, first_name, last_name, age, username, email, password, steam_id, is_moderator
                  FROM user
                  JOIN applications ON user.id = applications.playerId
                  WHERE applications.tournamentId=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                list.Add(new Player(
                    r.GetInt32(0),
                    r.GetString(1),
                    r.GetString(2),
                    r.GetInt32(3),
                    r.GetString(4),
                    r.GetString(5),
                    r.GetString(6),
                    r.GetString(7),
                    r.GetBoolean(8)
                ));
            }

            return list;
        }

        // ===========================================================
        // TEAMS
        // ===========================================================
        public void AddTeamTournamentApp(int teamId, int tournamentId)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "INSERT INTO team_applications (teamId, tournamentId) VALUES (@t, @tt)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@t", teamId);
            cmd.Parameters.AddWithValue("@tt", tournamentId);

            cmd.ExecuteNonQuery();
        }

        public List<int> GetTeamApplications(int tournamentId)
        {
            EnsureConnection();

            var list = new List<int>();

            var cmd = new MySqlCommand(
                "SELECT teamId FROM team_applications WHERE tournamentId=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", tournamentId);

            using var r = cmd.ExecuteReader();

            while (r.Read())
                list.Add(r.GetInt32(0));

            return list;
        }

        public void RemoveTeamTournamentApp(int teamId, int tournamentId)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "DELETE FROM team_applications WHERE teamId=@t AND tournamentId=@tt",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@t", teamId);
            cmd.Parameters.AddWithValue("@tt", tournamentId);

            cmd.ExecuteNonQuery();
        }
    }
}
