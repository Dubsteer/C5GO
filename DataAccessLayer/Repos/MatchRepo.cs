using LogicLayer;
using LogicLayer.Enums;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System.Data;
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

            // ✅ Updated query: uses match_date instead of datetime
            // Adjust table/column names to exactly match your DB schema in DataGrip
            var cmd = new MySqlCommand(@"
                SELECT 
                    m.id AS match_id,
                    m.tournamentId AS match_tournamentId,
                    m.player1Score AS match_player1Score,
                    m.player2Score AS match_player2Score,
                    m.match_date AS match_date,
                    m.status AS match_status,
                    u1.id AS user1_id,
                    u1.first_name AS user1_first_name,
                    u1.last_name AS user1_last_name,
                    u1.age AS user1_age,
                    u1.username AS user1_username,
                    u1.email AS user1_email,
                    u1.is_admin AS user1_is_admin,
                    u1.steam_id AS user1_steam_id,
                    u2.id AS user2_id,
                    u2.first_name AS user2_first_name,
                    u2.last_name AS user2_last_name,
                    u2.age AS user2_age,
                    u2.username AS user2_username,
                    u2.email AS user2_email,
                    u2.is_admin AS user2_is_admin,
                    u2.steam_id AS user2_steam_id
                FROM matches m
                JOIN user u1 ON m.user_id1 = u1.id
                JOIN user u2 ON m.user_id2 = u2.id;",
                conn.GetInnerConn());

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var player1 = new Player(
                            reader.GetInt32("user1_id"),
                            reader.GetString("user1_first_name"),
                            reader.GetString("user1_last_name"),
                            reader.GetInt32("user1_age"),
                            reader.GetString("user1_username"),
                            reader.GetString("user1_email"),
                            "0", // Password placeholder
                            reader.IsDBNull("user1_steam_id") ? "" : reader.GetString("user1_steam_id"),
                            reader.GetBoolean("user1_is_admin")
                        );

                        var player2 = new Player(
                            reader.GetInt32("user2_id"),
                            reader.GetString("user2_first_name"),
                            reader.GetString("user2_last_name"),
                            reader.GetInt32("user2_age"),
                            reader.GetString("user2_username"),
                            reader.GetString("user2_email"),
                            "0",
                            reader.IsDBNull("user2_steam_id") ? "" : reader.GetString("user2_steam_id"),
                            reader.GetBoolean("user2_is_admin")
                        );

                        matches.Add(new Match(
                            reader.GetInt32("match_id"),
                            reader.GetInt32("match_tournamentId"),
                            player1,
                            player2,
                            reader.GetInt32("match_player1Score"),
                            reader.GetInt32("match_player2Score"),
                            reader.GetDateTime("match_date"),
                            Enum.Parse<Status>(reader.GetString("match_status"))
                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }

            return matches;
        }

        public void AddMatch(Match match)
        {
            // ✅ also changed datetime -> match_date
            var cmd = new MySqlCommand(@"
                INSERT INTO matches
                (tournamentId, player1Score, player2Score, user_id1, user_id2, match_date, status)
                VALUES (@TOURNAMENTID, @PLAYER1SCORE, @PLAYER2SCORE, @USER_ID1, @USER_ID2, @MATCH_DATE, @STATUS)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("TOURNAMENTID", match.TournamentId);
            cmd.Parameters.AddWithValue("PLAYER1SCORE", match.Player1Score);
            cmd.Parameters.AddWithValue("PLAYER2SCORE", match.Player2Score);
            cmd.Parameters.AddWithValue("USER_ID1", match.User1.Id);
            cmd.Parameters.AddWithValue("USER_ID2", match.User2.Id);
            cmd.Parameters.AddWithValue("MATCH_DATE", match.MatchDate);
            cmd.Parameters.AddWithValue("STATUS", match.Status.ToString());

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }
        }

        public void RemoveMatch(Match match)
        {
            var cmd = new MySqlCommand("DELETE FROM matches WHERE id = @ID", conn.GetInnerConn());
            cmd.Parameters.AddWithValue("ID", match.Id);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }
        }

        public void UpdateMatch(Match match)
        {
            var cmd = new MySqlCommand(@"
                UPDATE matches
                SET tournamentId = @TOURNAMENTID,
                    player1Score = @PLAYER1SCORE,
                    player2Score = @PLAYER2SCORE,
                    user_id1 = @USER_ID1,
                    user_id2 = @USER_ID2,
                    match_date = @MATCH_DATE,
                    status = @STATUS
                WHERE id = @ID",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("ID", match.Id);
            cmd.Parameters.AddWithValue("TOURNAMENTID", match.TournamentId);
            cmd.Parameters.AddWithValue("PLAYER1SCORE", match.Player1Score);
            cmd.Parameters.AddWithValue("PLAYER2SCORE", match.Player2Score);
            cmd.Parameters.AddWithValue("USER_ID1", match.User1.Id);
            cmd.Parameters.AddWithValue("USER_ID2", match.User2.Id);
            cmd.Parameters.AddWithValue("MATCH_DATE", match.MatchDate);
            cmd.Parameters.AddWithValue("STATUS", match.Status.ToString());

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }
        }
    }
}
