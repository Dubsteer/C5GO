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

        public List<Tournament> GetAllTournaments()
        {
            EnsureConnection();

            var list = new List<Tournament>();

            var cmd = new MySqlCommand(@"
        SELECT id, name, description, status_int, 
               is_team, team_size_required
        FROM tournament", conn.GetInnerConn());

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Tournament(
                    reader.GetInt32("id"),
                    reader.GetString("name"),
                    reader.GetString("description"),
                    (Status)reader.GetInt32("status_int"),
                    reader.GetBoolean("is_team"),
                    reader.GetInt32("team_size_required")
                ));
            }

            return list;
        }


        public void AddTournament(Tournament t)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(@"
        INSERT INTO tournament
        (name, description, status_int, is_team, team_size_required)
        VALUES (@n, @d, @s, @team, @size)", conn.GetInnerConn());

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

            var cmd = new MySqlCommand(@"
        UPDATE tournament
        SET name=@n, description=@d, status_int=@s,
            is_team=@team, team_size_required=@size
        WHERE id=@id", conn.GetInnerConn());

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

            new MySqlCommand("DELETE FROM matches WHERE tournamentId=@id", conn.GetInnerConn())
            { Parameters = { new("@id", t.Id) } }.ExecuteNonQuery();

            new MySqlCommand("DELETE FROM applications WHERE tournamentId=@id", conn.GetInnerConn())
            { Parameters = { new("@id", t.Id) } }.ExecuteNonQuery();

            new MySqlCommand("DELETE FROM team_applications WHERE tournamentId=@id", conn.GetInnerConn())
            { Parameters = { new("@id", t.Id) } }.ExecuteNonQuery();

            new MySqlCommand("DELETE FROM tournament WHERE id=@id", conn.GetInnerConn())
            { Parameters = { new("@id", t.Id) } }.ExecuteNonQuery();
        }

        public void AddTournamentApp(Player p, Tournament t)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "INSERT INTO applications (tournamentId, playerId) VALUES (@tid, @pid)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@tid", t.Id);
            cmd.Parameters.AddWithValue("@pid", p.Id);

            cmd.ExecuteNonQuery();
        }

        public void RemovePlayerFromTournament(Player p, Tournament t)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "DELETE FROM applications WHERE tournamentId=@tid AND playerId=@pid",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@tid", t.Id);
            cmd.Parameters.AddWithValue("@pid", p.Id);

            cmd.ExecuteNonQuery();
        }

        public List<Player> GetAllPlayersInTournament(int tid)
        {
            EnsureConnection();

            var list = new List<Player>();

            var cmd = new MySqlCommand(@"
                SELECT user.id, first_name, last_name, age, username, email, password, steam_id, is_moderator
                FROM user
                JOIN applications ON user.id = applications.playerId
                WHERE applications.tournamentId=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", tid);

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

        // TEAM APPLICATIONS ---------
        public void AddTeamTournamentApp(int teamId, int tournamentId)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "INSERT INTO team_applications (teamId, tournamentId) VALUES (@team, @tour)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@team", teamId);
            cmd.Parameters.AddWithValue("@tour", tournamentId);

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
    }
}
