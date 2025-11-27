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
    SELECT 
        m.id AS match_id,
        m.tournamentId AS match_tournamentId,
        m.player1Score AS match_player1Score,
        m.player2Score AS match_player2Score,
        m.datetime AS match_datetime,
        m.status AS match_status,

        u1.id AS user1_id,
        u1.first_name AS user1_first_name,
        u1.last_name AS user1_last_name,
        u1.age AS user1_age,
        u1.username AS user1_username,
        u1.email AS user1_email,
        u1.password AS user1_password,
        u1.is_moderator AS user1_is_moderator,
        u1.steam_id AS user1_steam_id,

        u2.id AS user2_id,
        u2.first_name AS user2_first_name,
        u2.last_name AS user2_last_name,
        u2.age AS user2_age,
        u2.username AS user2_username,
        u2.email AS user2_email,
        u2.password AS user2_password,
        u2.is_moderator AS user2_is_moderator,
        u2.steam_id AS user2_steam_id

    FROM `match` m
    JOIN user u1 ON m.user_id1 = u1.id
    JOIN user u2 ON m.user_id2 = u2.id
", conn.GetInnerConn());


            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                var user1 = new User(
                    r.GetInt32("user1_id"),
                    r.GetString("user1_first_name"),
                    r.GetString("user1_last_name"),
                    r.GetInt32("user1_age"),
                    r.GetString("user1_username"),
                    r.GetString("user1_email"),
                    r.GetString("user1_password"),
                    r.GetBoolean("user1_is_moderator"),
                    r.GetString("user1_steam_id")
                );

                var user2 = new User(
                    r.GetInt32("user2_id"),
                    r.GetString("user2_first_name"),
                    r.GetString("user2_last_name"),
                    r.GetInt32("user2_age"),
                    r.GetString("user2_username"),
                    r.GetString("user2_email"),
                    r.GetString("user2_password"),
                    r.GetBoolean("user2_is_moderator"),
                    r.GetString("user2_steam_id")
                );

                matches.Add(new Match(
                    r.GetInt32("match_id"),
                    r.GetInt32("match_tournamentId"),
                    new Player(user1),
                    new Player(user2),
                    r.GetInt32("match_player1Score"),
                    r.GetInt32("match_player2Score"),
                    r.GetDateTime("match_datetime"),
                    (Status)Enum.Parse(typeof(Status), r.GetString("match_status"))
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
