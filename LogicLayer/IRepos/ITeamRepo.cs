using LogicLayer.Models;
using System.Collections.Generic;

namespace LogicLayer.IRepos
{
    public interface ITeamRepo
    {
        Team? GetTeamById(int id);
        Team? GetTeamByUser(int userId);
        List<Team> GetAllTeams();
        List<User> GetTeamMembers(int teamId);

        void CreateTeam(string name, int captainId);
        void AddPlayerToTeam(int teamId, int userId, string role, string status);

        void UpdatePlayerStatus(int teamId, int userId, string newStatus);
        void RemovePlayer(int teamId, int userId);

        void CreateJoinRequest(int teamId, int userId);
        void DeleteJoinRequest(int requestId);
        List<TeamJoinRequest> GetRequestsForTeam(int teamId);
        List<TeamJoinRequest> GetRequestsForUser(int userId);

        User? GetUserById(int userId);
        void DeleteTeam(int id);

    }
}
