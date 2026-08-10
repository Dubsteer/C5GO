using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace DataLayer.Repos
{
    public class TeamRepo : ITeamRepo
    {
        private readonly IConnection conn;

        public TeamRepo(IConnection conn)
        {
            this.conn = conn;
        }

        private void EnsureOpen()
        {
            if (conn.GetInnerConn().State != ConnectionState.Open)
                conn.Open();
        }


        public User? GetUserById(int id)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"SELECT id, first_name, last_name, username, email, is_moderator, steam_id 
                  FROM user WHERE id=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return new User
            {
                Id = r.GetInt32("id"),
                Firstname = r.GetString("first_name"),
                Lastname = r.GetString("last_name"),
                Username = r.GetString("username"),
                Gmail = r.GetString("email"),
                IsAdmin = r.GetBoolean("is_moderator"),
                SteamId = r.IsDBNull("steam_id") ? "0" : r.GetString("steam_id")
            };
        }


        public Team? GetTeamById(int id)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"SELECT 
                      t.id, 
                      t.name,
                      u.id AS captain_id,
                      u.first_name,
                      u.last_name,
                      u.username
                  FROM team t
                  JOIN user u ON t.captain_id = u.id
                  WHERE t.id=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            var captain = new User(r.GetInt32("captain_id"))
            {
                Firstname = r.GetString("first_name"),
                Lastname = r.GetString("last_name"),
                Username = r.GetString("username")
            };

            return new Team(r.GetInt32("id"), r.GetString("name"), captain);
        }

        public Team? GetTeamByUser(int userId)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"SELECT 
                      t.id, 
                      t.name,
                      u.id AS captain_id,
                      u.first_name,
                      u.last_name,
                      u.username
                  FROM team_player tp
                  JOIN team t ON tp.team_id = t.id
                  JOIN user u ON t.captain_id = u.id
                  WHERE tp.user_id=@uid AND tp.status='Approved'",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@uid", userId);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            var captain = new User(r.GetInt32("captain_id"))
            {
                Firstname = r.GetString("first_name"),
                Lastname = r.GetString("last_name"),
                Username = r.GetString("username")
            };

            return new Team(r.GetInt32("id"), r.GetString("name"), captain);
        }

        public List<Team> GetAllTeams()
        {
            EnsureOpen();

            var list = new List<Team>();

            var cmd = new MySqlCommand(
                @"SELECT 
                      t.id, 
                      t.name,
                      u.id AS captain_id,
                      u.first_name,
                      u.last_name,
                      u.username
                  FROM team t
                  JOIN user u ON t.captain_id = u.id",
                conn.GetInnerConn());

            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                list.Add(new Team(
                    r.GetInt32("id"),
                    r.GetString("name"),
                    new User(r.GetInt32("captain_id"))
                    {
                        Firstname = r.GetString("first_name"),
                        Lastname = r.GetString("last_name"),
                        Username = r.GetString("username")
                    }
                ));
            }

            return list;
        }

        public List<User> GetTeamMembers(int teamId)
        {
            EnsureOpen();

            var list = new List<User>();

            var cmd = new MySqlCommand(
                @"SELECT 
                      u.id, u.first_name, u.last_name, u.username, u.steam_id
                  FROM team_player tp
                  JOIN user u ON tp.user_id = u.id
                  WHERE tp.team_id=@id AND tp.status='Approved'",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", teamId);

            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                list.Add(new User
                {
                    Id = r.GetInt32("id"),
                    Firstname = r.GetString("first_name"),
                    Lastname = r.GetString("last_name"),
                    Username = r.GetString("username"),
                    SteamId = r.IsDBNull("steam_id") ? "0" : r.GetString("steam_id")
                });
            }

            return list;
        }


        public void CreateTeam(string name, int captainId)
        {
            EnsureOpen();

            var captain = GetUserById(captainId);
            if (captain == null)
                throw new Exception("Captain does not exist.");

            if (string.IsNullOrWhiteSpace(captain.SteamId) || captain.SteamId == "0")
                throw new Exception("Captain must have a SteamID.");

            var cmd = new MySqlCommand(
                @"INSERT INTO team (name, captain_id) VALUES (@n, @c)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@c", captainId);
            cmd.ExecuteNonQuery();

            int teamId = (int)cmd.LastInsertedId;

            AddPlayerToTeam(teamId, captainId, "Captain", "Approved");
        }

        public void AddPlayerToTeam(int teamId, int userId, string role, string status)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"INSERT INTO team_player (team_id, user_id, role, status)
                  VALUES (@t, @u, @r, @s)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@t", teamId);
            cmd.Parameters.AddWithValue("@u", userId);
            cmd.Parameters.AddWithValue("@r", role);
            cmd.Parameters.AddWithValue("@s", status);

            cmd.ExecuteNonQuery();
        }


        public void CreateJoinRequest(int teamId, int userId)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"INSERT INTO team_join_request (team_id, user_id)
                  VALUES (@t, @u)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@t", teamId);
            cmd.Parameters.AddWithValue("@u", userId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteJoinRequest(int requestId)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"DELETE FROM team_join_request WHERE id=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", requestId);
            cmd.ExecuteNonQuery();
        }

        public List<TeamJoinRequest> GetRequestsForTeam(int teamId)
        {
            EnsureOpen();

            var list = new List<TeamJoinRequest>();

            var cmd = new MySqlCommand(
                @"SELECT r.id, r.team_id, r.user_id, r.requested_at, u.username
                  FROM team_join_request r
                  JOIN user u ON r.user_id=u.id
                  WHERE r.team_id=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", teamId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new TeamJoinRequest
                {
                    Id = r.GetInt32("id"),
                    TeamId = r.GetInt32("team_id"),
                    UserId = r.GetInt32("user_id"),
                    RequestedAt = r.GetDateTime("requested_at"),
                    User = new User { Id = r.GetInt32("user_id"), Username = r.GetString("username") }
                });
            }

            return list;
        }

        public List<TeamJoinRequest> GetRequestsForUser(int userId)
        {
            EnsureOpen();

            var list = new List<TeamJoinRequest>();

            var cmd = new MySqlCommand(
                @"SELECT id, team_id, user_id, requested_at
                  FROM team_join_request WHERE user_id=@uid",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@uid", userId);

            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                list.Add(new TeamJoinRequest
                {
                    Id = r.GetInt32("id"),
                    TeamId = r.GetInt32("team_id"),
                    UserId = r.GetInt32("user_id"),
                    RequestedAt = r.GetDateTime("requested_at")
                });
            }

            return list;
        }

        public void DeleteTeam(int id)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"DELETE FROM team WHERE id=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }


        public void RemovePlayer(int teamId, int userId)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"DELETE FROM team_player WHERE team_id=@t AND user_id=@u",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@t", teamId);
            cmd.Parameters.AddWithValue("@u", userId);
            cmd.ExecuteNonQuery();
        }

        public void UpdatePlayerStatus(int teamId, int userId, string newStatus)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"UPDATE team_player SET status=@s WHERE team_id=@t AND user_id=@u",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@s", newStatus);
            cmd.Parameters.AddWithValue("@t", teamId);
            cmd.Parameters.AddWithValue("@u", userId);
            cmd.ExecuteNonQuery();
        }
    }
}
