using LogicLayer;
using LogicLayer.Enums;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace DataLayer.Repos
{
    public class MatchRepo : IMatchRepo
    {
        private readonly IConnection conn;

        public MatchRepo(IConnection conn)
        {
            this.conn = conn;
        }

        private void EnsureOpen()
        {
            if (conn.GetInnerConn().State != ConnectionState.Open)
                conn.Open();
        }

        public List<Match> GetAllMatches()
        {
            EnsureOpen();

            var list = new List<Match>();

            var cmd = new MySqlCommand(@"
                SELECT 
                    m.id,
                    m.tournamentId,
                    m.player1Score,
                    m.player2Score,
                    m.match_date,
                    m.status_int,

                    u1.id AS u1_id, u1.first_name AS u1_first_name,
                    u1.last_name AS u1_last_name, u1.age AS u1_age,
                    u1.username AS u1_username, u1.email AS u1_email,
                    u1.is_moderator AS u1_is_moderator, u1.steam_id AS u1_steam_id,

                    u2.id AS u2_id, u2.first_name AS u2_first_name,
                    u2.last_name AS u2_last_name, u2.age AS u2_age,
                    u2.username AS u2_username, u2.email AS u2_email,
                    u2.is_moderator AS u2_is_moderator, u2.steam_id AS u2_steam_id
                FROM matches m
                JOIN user u1 ON m.user_id1 = u1.id
                JOIN user u2 ON m.user_id2 = u2.id
            ", conn.GetInnerConn());

            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    var p1 = new Player(new User(
                        r.GetInt32("u1_id"),
                        r.GetString("u1_first_name"),
                        r.GetString("u1_last_name"),
                        r.GetInt32("u1_age"),
                        r.GetString("u1_username"),
                        r.GetString("u1_email"),
                        string.Empty,
                        r.GetBoolean("u1_is_moderator"),
                        r.GetString("u1_steam_id")
                    ));

                    var p2 = new Player(new User(
                        r.GetInt32("u2_id"),
                        r.GetString("u2_first_name"),
                        r.GetString("u2_last_name"),
                        r.GetInt32("u2_age"),
                        r.GetString("u2_username"),
                        r.GetString("u2_email"),
                        string.Empty,
                        r.GetBoolean("u2_is_moderator"),
                        r.GetString("u2_steam_id")
                    ));

                    list.Add(new Match(
                        r.GetInt32("id"),
                        r.GetInt32("tournamentId"),
                        p1,
                        p2,
                        r.GetInt32("player1Score"),
                        r.GetInt32("player2Score"),
                        r.GetDateTime("match_date"),
                        (Status)r.GetInt32("status_int")
                    ));
                }
            }

            return list;
        }

        public void AddMatch(Match match)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(@"
                INSERT INTO matches
                (tournamentId, user_id1, user_id2, player1Score, player2Score, match_date, status_int)
                VALUES (@t,@u1,@u2,@s1,@s2,@d,@st)
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@t", match.TournamentId);
            cmd.Parameters.AddWithValue("@u1", match.User1.Id);
            cmd.Parameters.AddWithValue("@u2", match.User2.Id);
            cmd.Parameters.AddWithValue("@s1", match.Player1Score);
            cmd.Parameters.AddWithValue("@s2", match.Player2Score);
            cmd.Parameters.AddWithValue("@d", match.MatchDate);
            cmd.Parameters.AddWithValue("@st", (int)match.Status);

            cmd.ExecuteNonQuery();
        }

        public void UpdateMatch(Match match)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(@"
                UPDATE matches SET
                    user_id1=@u1,
                    user_id2=@u2,
                    player1Score=@s1,
                    player2Score=@s2,
                    match_date=@d,
                    status_int=@st
                WHERE id=@id
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", match.Id);
            cmd.Parameters.AddWithValue("@u1", match.User1.Id);
            cmd.Parameters.AddWithValue("@u2", match.User2.Id);
            cmd.Parameters.AddWithValue("@s1", match.Player1Score);
            cmd.Parameters.AddWithValue("@s2", match.Player2Score);
            cmd.Parameters.AddWithValue("@d", match.MatchDate);
            cmd.Parameters.AddWithValue("@st", (int)match.Status);

            cmd.ExecuteNonQuery();
        }

        public void RemoveMatch(Match match)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                "DELETE FROM matches WHERE id=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", match.Id);
            cmd.ExecuteNonQuery();
        }
    }
}
