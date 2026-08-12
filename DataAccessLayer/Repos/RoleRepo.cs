using LogicLayer.Enums;
using LogicLayer.IRepos;
using MySql.Data.MySqlClient;
using System.Data;

namespace DataLayer.Repos;

public class RoleRepo : IRoleRepo
{
    private readonly IConnection conn;

    public RoleRepo(IConnection conn)
    {
        this.conn = conn;
    }

    public IReadOnlyList<PlatformRole> GetRolesForUser(int userId)
    {
        EnsureConnection();

        using var command = new MySqlCommand(@"
            SELECT role_id
            FROM user_role
            WHERE user_id = @USER_ID
            ORDER BY role_id
        ", conn.Connection);
        command.Parameters.AddWithValue("@USER_ID", userId);

        var roles = new List<PlatformRole>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
            roles.Add((PlatformRole)reader.GetByte("role_id"));

        return roles;
    }

    public bool AssignRole(
        int userId,
        PlatformRole role,
        int? assignedBy,
        string? reason)
    {
        EnsureConnection();
        using var transaction = conn.Connection.BeginTransaction();

        if (RoleExists(userId, role, transaction))
        {
            transaction.Commit();
            return false;
        }

        using (var command = new MySqlCommand(@"
            INSERT INTO user_role (user_id, role_id, assigned_by, assigned_at, reason)
            VALUES (@USER_ID, @ROLE_ID, @ASSIGNED_BY, UTC_TIMESTAMP(), @REASON)
        ", conn.Connection, transaction))
        {
            AddRoleParameters(command, userId, role, assignedBy, reason);
            command.ExecuteNonQuery();
        }

        AddAuditEntry(
            userId,
            role,
            RoleAuditAction.Assigned,
            assignedBy,
            reason,
            transaction);

        transaction.Commit();
        return true;
    }

    public bool RevokeRole(
        int userId,
        PlatformRole role,
        int? performedBy,
        string? reason)
    {
        EnsureConnection();
        using var transaction = conn.Connection.BeginTransaction();

        using var command = new MySqlCommand(@"
            DELETE FROM user_role
            WHERE user_id = @USER_ID AND role_id = @ROLE_ID
        ", conn.Connection, transaction);
        command.Parameters.AddWithValue("@USER_ID", userId);
        command.Parameters.AddWithValue("@ROLE_ID", (byte)role);

        if (command.ExecuteNonQuery() == 0)
        {
            transaction.Commit();
            return false;
        }

        if (role == PlatformRole.Admin &&
            !RoleExists(userId, PlatformRole.Owner, transaction))
        {
            ClearLegacyAdministratorFlag(userId, transaction);
        }

        AddAuditEntry(
            userId,
            role,
            RoleAuditAction.Revoked,
            performedBy,
            reason,
            transaction);

        transaction.Commit();
        return true;
    }

    private bool RoleExists(
        int userId,
        PlatformRole role,
        MySqlTransaction transaction)
    {
        using var command = new MySqlCommand(@"
            SELECT EXISTS(
                SELECT 1
                FROM user_role
                WHERE user_id = @USER_ID AND role_id = @ROLE_ID
            )
        ", conn.Connection, transaction);
        command.Parameters.AddWithValue("@USER_ID", userId);
        command.Parameters.AddWithValue("@ROLE_ID", (byte)role);
        return Convert.ToInt32(command.ExecuteScalar()) == 1;
    }

    private static void AddRoleParameters(
        MySqlCommand command,
        int userId,
        PlatformRole role,
        int? assignedBy,
        string? reason)
    {
        command.Parameters.AddWithValue("@USER_ID", userId);
        command.Parameters.AddWithValue("@ROLE_ID", (byte)role);
        command.Parameters.AddWithValue(
            "@ASSIGNED_BY",
            assignedBy.HasValue ? assignedBy.Value : DBNull.Value);
        command.Parameters.AddWithValue(
            "@REASON",
            string.IsNullOrWhiteSpace(reason) ? DBNull.Value : reason);
    }

    private void AddAuditEntry(
        int userId,
        PlatformRole role,
        RoleAuditAction action,
        int? performedBy,
        string? reason,
        MySqlTransaction transaction)
    {
        using var command = new MySqlCommand(@"
            INSERT INTO role_assignment_audit
                (user_id, role_id, action_type, performed_by, reason, created_at)
            VALUES
                (@USER_ID, @ROLE_ID, @ACTION_TYPE, @PERFORMED_BY, @REASON, UTC_TIMESTAMP())
        ", conn.Connection, transaction);
        command.Parameters.AddWithValue("@USER_ID", userId);
        command.Parameters.AddWithValue("@ROLE_ID", (byte)role);
        command.Parameters.AddWithValue("@ACTION_TYPE", (byte)action);
        command.Parameters.AddWithValue(
            "@PERFORMED_BY",
            performedBy.HasValue ? performedBy.Value : DBNull.Value);
        command.Parameters.AddWithValue(
            "@REASON",
            string.IsNullOrWhiteSpace(reason) ? DBNull.Value : reason);
        command.ExecuteNonQuery();
    }

    private void ClearLegacyAdministratorFlag(
        int userId,
        MySqlTransaction transaction)
    {
        using var command = new MySqlCommand(@"
            UPDATE `user`
            SET is_moderator = 0
            WHERE id = @USER_ID
        ", conn.Connection, transaction);
        command.Parameters.AddWithValue("@USER_ID", userId);
        command.ExecuteNonQuery();
    }

    private void EnsureConnection()
    {
        if (conn.Connection.State != ConnectionState.Open)
            conn.Open();
    }
}
