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
            if (conn.Connection.State != System.Data.ConnectionState.Open)
                conn.Open();
        }

        private static bool IsNull(MySqlDataReader reader, string column)
        {
            return reader.IsDBNull(reader.GetOrdinal(column));
        }

        public List<Tournament> GetAllTournaments()
        {
            EnsureConnection();

            var list = new List<Tournament>();

            var cmd = new MySqlCommand(
                @"SELECT id, name, description, status_int, is_team, team_size_required 
                  FROM tournament",
                conn.Connection);

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
            r.Close();

            foreach (var t in list)
            {
                t.PlayersCount = GetPlayersCount(t.Id);
                t.TeamsCount = GetTeamsCount(t.Id);
                t.MatchesCount = GetMatchesCount(t.Id);
                t.CanLeave = false;
            }

            return list;
        }

        public Tournament? GetTournamentById(int id)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                @"SELECT id, name, description, status_int, is_team, team_size_required
                  FROM tournament WHERE id=@id",
                conn.Connection);

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

            t.PlayersCount = GetPlayersCount(t.Id);
            t.TeamsCount = GetTeamsCount(t.Id);
            t.MatchesCount = GetMatchesCount(t.Id);
            t.CanLeave = false;

            return t;
        }

        public void AddTournament(Tournament t)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                @"INSERT INTO tournament 
                  (name, description, status_int, is_team, team_size_required)
                  VALUES (@n, @d, @s, @team, @size)",
                conn.Connection);

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
                conn.Connection);

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

            using var transaction = conn.Connection.BeginTransaction();
            try
            {
                foreach (var table in new[] { "matches", "team_matches", "applications", "team_applications" })
                {
                    using var childCommand = new MySqlCommand(
                        $"DELETE FROM {table} WHERE tournamentId=@id",
                        conn.Connection,
                        transaction);
                    childCommand.Parameters.AddWithValue("@id", t.Id);
                    childCommand.ExecuteNonQuery();
                }

                using var tournamentCommand = new MySqlCommand(
                    "DELETE FROM tournament WHERE id=@id",
                    conn.Connection,
                    transaction);
                tournamentCommand.Parameters.AddWithValue("@id", t.Id);
                tournamentCommand.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private int GetPlayersCount(int tournamentId)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM applications WHERE tournamentId=@id",
                conn.Connection);

            cmd.Parameters.AddWithValue("@id", tournamentId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private int GetTeamsCount(int tournamentId)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM team_applications WHERE tournamentId=@id",
                conn.Connection);

            cmd.Parameters.AddWithValue("@id", tournamentId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private int GetMatchesCount(int tournamentId)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                @"SELECT 
                      (SELECT COUNT(*) FROM matches WHERE tournamentId=@id) +
                      (SELECT COUNT(*) FROM team_matches WHERE tournamentId=@id)",
                conn.Connection);

            cmd.Parameters.AddWithValue("@id", tournamentId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void AddTournamentApp(Player p, Tournament t)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "INSERT INTO applications (tournamentId, playerId) VALUES (@t, @p)",
                conn.Connection);

            cmd.Parameters.AddWithValue("@t", t.Id);
            cmd.Parameters.AddWithValue("@p", p.Id);

            cmd.ExecuteNonQuery();
        }

        public void RemovePlayerFromTournament(Player p, Tournament t)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "DELETE FROM applications WHERE tournamentId=@t AND playerId=@p",
                conn.Connection);

            cmd.Parameters.AddWithValue("@t", t.Id);
            cmd.Parameters.AddWithValue("@p", p.Id);

            cmd.ExecuteNonQuery();
        }

        public List<Player> GetAllPlayersInTournament(int id)
        {
            EnsureConnection();

            var list = new List<Player>();

            var cmd = new MySqlCommand(
                @"SELECT user.id, first_name, last_name, age, username, email, steam_id, is_moderator
                  FROM user
                  JOIN applications ON user.id = applications.playerId
                  WHERE applications.tournamentId=@id",
                conn.Connection);

            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                list.Add(new Player(
                    r.GetInt32("id"),
                    IsNull(r, "first_name") ? string.Empty : r.GetString("first_name"),
                    IsNull(r, "last_name") ? string.Empty : r.GetString("last_name"),
                    IsNull(r, "age") ? 0 : r.GetInt32("age"),
                    IsNull(r, "username") ? string.Empty : r.GetString("username"),
                    IsNull(r, "email") ? string.Empty : r.GetString("email"),
                    string.Empty,
                    IsNull(r, "steam_id") ? string.Empty : r.GetString("steam_id"),
                    !IsNull(r, "is_moderator") && r.GetBoolean("is_moderator")
                ));
            }

            return list;
        }

        public void AddTeamTournamentApp(int teamId, int tournamentId)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "INSERT INTO team_applications (teamId, tournamentId) VALUES (@t, @tt)",
                conn.Connection);

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
                conn.Connection);

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
                conn.Connection);

            cmd.Parameters.AddWithValue("@t", teamId);
            cmd.Parameters.AddWithValue("@tt", tournamentId);

            cmd.ExecuteNonQuery();
        }
    }
}
