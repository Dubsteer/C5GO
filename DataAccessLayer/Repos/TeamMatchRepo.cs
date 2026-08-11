using LogicLayer;
using LogicLayer.Enums;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace DataLayer.Repos
{
    public class TeamMatchRepo : ITeamMatchRepo
    {
        private readonly IConnection conn;

        public TeamMatchRepo(IConnection conn)
        {
            this.conn = conn;
        }

        private void EnsureOpen()
        {
            if (conn.Connection.State != ConnectionState.Open)
                conn.Open();
        }

        public List<TeamMatch> GetAllTeamMatches()
        {
            EnsureOpen();

            var list = new List<TeamMatch>();

            var cmd = new MySqlCommand(@"
                SELECT 
                    tm.id,
                    tm.tournamentId,
                    tm.team_id1 AS team1Id,
                    tm.team_id2 AS team2Id,
                    tm.team1_score,
                    tm.team2_score,
                    tm.match_date,
                    tm.status_int,
                    t1.name AS team1Name,
                    t2.name AS team2Name
                FROM team_matches tm
                JOIN team t1 ON tm.team_id1 = t1.id
                JOIN team t2 ON tm.team_id2 = t2.id
            ", conn.Connection);

            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    var t1 = new Team(
                        r.GetInt32("team1Id"),
                        r.GetString("team1Name"),
                        null
                    );

                    var t2 = new Team(
                        r.GetInt32("team2Id"),
                        r.GetString("team2Name"),
                        null
                    );

                    list.Add(new TeamMatch(
                        r.GetInt32("id"),
                        r.GetInt32("tournamentId"),
                        t1,
                        t2,
                        r.GetInt32("team1_score"),
                        r.GetInt32("team2_score"),
                        r.GetDateTime("match_date"),
                        (Status)r.GetInt32("status_int")
                    ));
                }
            }

            return list;
        }

        public void AddTeamMatch(TeamMatch match)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(@"
                INSERT INTO team_matches 
                (tournamentId, team_id1, team_id2, team1_score, team2_score, match_date, status_int)
                VALUES (@tid, @t1, @t2, @s1, @s2, @date, @status)
            ", conn.Connection);

            cmd.Parameters.AddWithValue("@tid", match.TournamentId);
            cmd.Parameters.AddWithValue("@t1", match.Team1.Id);
            cmd.Parameters.AddWithValue("@t2", match.Team2.Id);
            cmd.Parameters.AddWithValue("@s1", match.Team1Score);
            cmd.Parameters.AddWithValue("@s2", match.Team2Score);
            cmd.Parameters.AddWithValue("@date", match.MatchDate);
            cmd.Parameters.AddWithValue("@status", (int)match.Status);

            cmd.ExecuteNonQuery();
        }

        public void UpdateTeamMatch(TeamMatch match)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(@"
                UPDATE team_matches SET
                    team1_score=@s1,
                    team2_score=@s2,
                    team_id1=@t1,
                    team_id2=@t2,
                    match_date=@date,
                    status_int=@status
                WHERE id=@id
            ", conn.Connection);

            cmd.Parameters.AddWithValue("@id", match.Id);
            cmd.Parameters.AddWithValue("@s1", match.Team1Score);
            cmd.Parameters.AddWithValue("@s2", match.Team2Score);
            cmd.Parameters.AddWithValue("@t1", match.Team1.Id);
            cmd.Parameters.AddWithValue("@t2", match.Team2.Id);
            cmd.Parameters.AddWithValue("@date", match.MatchDate);
            cmd.Parameters.AddWithValue("@status", (int)match.Status);

            cmd.ExecuteNonQuery();
        }

        public void RemoveTeamMatch(TeamMatch match)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                "DELETE FROM team_matches WHERE id=@id",
                conn.Connection);

            cmd.Parameters.AddWithValue("@id", match.Id);
            cmd.ExecuteNonQuery();
        }
    }
}
