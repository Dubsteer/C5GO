using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Models;
using LogicLayer.Services;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace DataLayer.Repos
{
    public class PlayerRepo : IPlayerRepo
    {
        private readonly IConnection conn;

        public PlayerRepo(IConnection conn)
        {
            this.conn = conn;
        }

        private void EnsureConnection()
        {
            if (conn.GetInnerConn().State != ConnectionState.Open)
                conn.Open();
        }

        private string? SafeString(MySqlDataReader reader, string column)
        {
            return reader.IsDBNull(column) ? null : reader.GetString(column);
        }

        private int SafeInt(MySqlDataReader reader, string column)
        {
            return reader.IsDBNull(column) ? 0 : reader.GetInt32(column);
        }

        private bool SafeBool(MySqlDataReader reader, string column)
        {
            return !reader.IsDBNull(column) && reader.GetBoolean(column);
        }

        public void InitializeRole(Player player)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "UPDATE user SET steam_id = @steam_id WHERE id = @id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@steam_id", player.SteamId);
            cmd.Parameters.AddWithValue("@id", player.Id);

            cmd.ExecuteNonQuery();
        }

        public List<Player> GetAllPlayers()
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                @"SELECT id, first_name, last_name, age, username, email,
                         is_moderator, steam_id
                  FROM user
                  WHERE steam_id IS NOT NULL AND steam_id != '0'",
                conn.GetInnerConn());

            var list = new List<Player>();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (!SteamIdParser.TryNormalize(SafeString(reader, "steam_id"), out var steamId) ||
                    steamId == null)
                {
                    continue;
                }

                var u = new User(
                    reader.GetInt32("id"),
                    SafeString(reader, "first_name") ?? "",
                    SafeString(reader, "last_name") ?? "",
                    SafeInt(reader, "age"),
                    SafeString(reader, "username") ?? "",
                    SafeString(reader, "email") ?? "",
                    string.Empty,
                    SafeBool(reader, "is_moderator"),
                    steamId
                );

                list.Add(new Player(u));
            }

            return list;
        }

        public Player? GetPlayer(User user)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                @"SELECT id, first_name, last_name, age, username, email,
                         is_moderator, steam_id
                  FROM user
                  WHERE id=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", user.Id);

            using var r = cmd.ExecuteReader();
            if (!r.Read())
                return null;

            var steamId = SafeString(r, "steam_id");

            if (!SteamIdParser.TryNormalize(steamId, out var normalizedSteamId) ||
                normalizedSteamId == null)
                return null;

            var u = new User(
                r.GetInt32("id"),
                SafeString(r, "first_name") ?? "",
                SafeString(r, "last_name") ?? "",
                SafeInt(r, "age"),
                SafeString(r, "username") ?? "",
                SafeString(r, "email") ?? "",
                string.Empty,
                SafeBool(r, "is_moderator"),
                normalizedSteamId
            );

            return new Player(u);
        }

        public void AddPlayerToTournament(Player player, Tournament tournament)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "INSERT INTO applications (tournamentId, playerid) VALUES (@tid, @pid)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@tid", tournament.Id);
            cmd.Parameters.AddWithValue("@pid", player.Id);
            cmd.ExecuteNonQuery();
        }

        public bool DeletePlayerRole(int userId)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "UPDATE user SET steam_id = NULL WHERE id=@id AND steam_id IS NOT NULL AND steam_id != '0'",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", userId);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
