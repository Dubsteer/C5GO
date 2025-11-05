using LogicLayer;
using LogicLayer.Enums;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace DataLayer.Repos
{
    public class MatchRepo : IMatchRepo
    {
        private readonly IConnection conn;

        public MatchRepo(IConnection conn)
        {
            this.conn = conn;
        }

        public List<Match> GetAllMatches()
        {
            var matches = new List<Match>();

            var cmd = new MySqlCommand(@"
                SELECT m.*, 
                       u1.*, u2.*
                FROM matches m
                LEFT JOIN user u1 ON m.user_id1 = u1.id
                LEFT JOIN user u2 ON m.user_id2 = u2.id;", conn.GetInnerConn());

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var p1 = new Player(
                    reader.GetInt32("user_id1"),
                    reader.GetString("first_name"),
                    reader.GetString("last_name"),
                    21,
                    reader.GetString("username"),
                    reader.GetString("email"),
                    "0",
                    "",
                    reader.GetBoolean("is_moderator")
                );

                var p2 = new Player(
                    reader.GetInt32("user_id2"),
                    reader.GetString("first_name"),
                    reader.GetString("last_name"),
                    21,
                    reader.GetString("username"),
                    reader.GetString("email"),
                    "0",
                    "",
                    reader.GetBoolean("is_moderator")
                );

                matches.Add(new Match(
                    reader.GetInt32("id"),
                    reader.GetInt32("tournamentId"),
                    p1,
                    p2,
                    reader.GetInt32("player1Score"),
                    reader.GetInt32("player2Score"),
                    reader.GetDateTime("match_date"),
                    (Status)reader.GetInt32("status_int")
                ));
            }

            return matches;
        }

        public void AddMatch(Match match)
        {
            var cmd = new MySqlCommand(@"
                INSERT INTO matches
                (tournamentId, player1Score, player2Score, user_id1, user_id2, match_date, status_int)
                VALUES (@tid,@p1,@p2,@u1,@u2,@date,@s)", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@tid", match.TournamentId);
            cmd.Parameters.AddWithValue("@p1", match.Player1Score);
            cmd.Parameters.AddWithValue("@p2", match.Player2Score);
            cmd.Parameters.AddWithValue("@u1", match.User1.Id);
            cmd.Parameters.AddWithValue("@u2", match.User2.Id);
            cmd.Parameters.AddWithValue("@date", match.MatchDate);
            cmd.Parameters.AddWithValue("@s", (int)match.Status);

            cmd.ExecuteNonQuery();
        }

        public void RemoveMatch(Match match)
        {
            var cmd = new MySqlCommand("DELETE FROM matches WHERE id=@id", conn.GetInnerConn());
            cmd.Parameters.AddWithValue("@id", match.Id);
            cmd.ExecuteNonQuery();
        }

        public void UpdateMatch(Match match)
        {
            var cmd = new MySqlCommand(@"
                UPDATE matches SET 
                player1Score=@p1, player2Score=@p2, user_id1=@u1, user_id2=@u2,
                match_date=@date, status_int=@s WHERE id=@id", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", match.Id);
            cmd.Parameters.AddWithValue("@p1", match.Player1Score);
            cmd.Parameters.AddWithValue("@p2", match.Player2Score);
            cmd.Parameters.AddWithValue("@u1", match.User1.Id);
            cmd.Parameters.AddWithValue("@u2", match.User2.Id);
            cmd.Parameters.AddWithValue("@date", match.MatchDate);
            cmd.Parameters.AddWithValue("@s", (int)match.Status);

            cmd.ExecuteNonQuery();
        }
    }
}
