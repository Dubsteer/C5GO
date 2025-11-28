using LogicLayer.Models;
using System.Collections.Generic;

namespace LogicLayer.IRepos
{
    public interface ITeamRepo
    {
        // TEAM READ
        Team GetTeamById(int id);
        Team GetTeamByUser(int userId);
        List<Team> GetAllTeams();
        List<User> GetTeamMembers(int teamId);

        // TEAM CREATE + ADD PLAYER
        void CreateTeam(string name, int captainId);
        void AddPlayerToTeam(int teamId, int userId, string role, string status);

        // PLAYER STATUS + REMOVE
        void UpdatePlayerStatus(int teamId, int userId, string newStatus);
        void RemovePlayer(int teamId, int userId);

        // JOIN REQUESTS
        void CreateJoinRequest(int teamId, int userId);
        void DeleteJoinRequest(int requestId);
        List<TeamJoinRequest> GetRequestsForTeam(int teamId);
        List<TeamJoinRequest> GetRequestsForUser(int userId);

        // EXTRA FOR MANAGER
        User GetUserById(int userId);
        void DeleteTeam(int id);

        // ADMIN TOOLS
        List<User> GetUsersWithoutTeam();
        void AddUserToTeam_AdminOverride(int teamId, int userId);

        // TEAM MEMBERS
        List<User> GetUsersInTeam(int teamId);
    }
}
