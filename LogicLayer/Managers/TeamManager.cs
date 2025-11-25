using LogicLayer.IRepos;
using LogicLayer.Models;
using System;
using System.Collections.Generic;

namespace LogicLayer.Managers
{
    public class TeamManager
    {
        private readonly ITeamRepo teamRepo;

        public TeamManager(ITeamRepo repo)
        {
            teamRepo = repo;
        }

        public Team GetTeamOfUser(int userId)
        {
            return teamRepo.GetTeamByUser(userId);
        }

        public Team GetTeam(int id)
        {
            var t = teamRepo.GetTeamById(id);
            if (t != null)
                t.Members = teamRepo.GetTeamMembers(id);
            return t;
        }

        public List<Team> GetAllTeams()
        {
            var teams = teamRepo.GetAllTeams();
            foreach (var t in teams)
                t.Members = teamRepo.GetTeamMembers(t.Id);
            return teams;
        }

        public List<TeamJoinRequest> GetJoinRequests(int teamId)
        {
            return teamRepo.GetRequestsForTeam(teamId);
        }

        public List<TeamJoinRequest> GetRequestsForUser(int userId)
        {
            return teamRepo.GetRequestsForUser(userId);
        }

        public void CreateTeam(string name, int captainId)
        {
            teamRepo.CreateTeam(name, captainId);
        }

        public void RequestJoinTeam(int teamId, int userId)
        {
            if (teamRepo.GetTeamByUser(userId) != null)
                throw new Exception("Already in a team.");

            if (teamRepo.GetRequestsForUser(userId).Exists(r => r.TeamId == teamId))
                throw new Exception("Request already exists.");

            teamRepo.CreateJoinRequest(teamId, userId);
        }

        // ---------------------------------------------------------
        // APPROVE
        // ---------------------------------------------------------

        public void ApproveRequest(int requestId, int captainId)
        {
            var captainTeam = teamRepo.GetTeamByUser(captainId);
            if (captainTeam == null)
                throw new Exception("Captain not in a team.");

            if (captainTeam.Captain.Id != captainId)
                throw new Exception("Only captain can approve requests.");

            var requests = teamRepo.GetRequestsForTeam(captainTeam.Id);
            var req = requests.Find(r => r.Id == requestId);

            if (req == null)
                throw new Exception("Request not found.");

            teamRepo.AddPlayerToTeam(captainTeam.Id, req.UserId, "Member", "Approved");
            teamRepo.DeleteJoinRequest(requestId);
        }

        // ---------------------------------------------------------
        // REJECT
        // ---------------------------------------------------------

        public void RejectRequest(int requestId, int captainId)
        {
            var captainTeam = teamRepo.GetTeamByUser(captainId);
            if (captainTeam == null)
                throw new Exception("Captain not in a team.");

            if (captainTeam.Captain.Id != captainId)
                throw new Exception("Only captain can reject requests.");

            var requests = teamRepo.GetRequestsForTeam(captainTeam.Id);
            var req = requests.Find(r => r.Id == requestId);

            if (req == null)
                throw new Exception("Request not found.");

            teamRepo.DeleteJoinRequest(requestId);
        }

        // ---------------------------------------------------------
        // LEAVE TEAM
        // ---------------------------------------------------------

        public void LeaveTeam(int userId)
        {
            var team = teamRepo.GetTeamByUser(userId);

            if (team == null)
                throw new Exception("Not in a team.");

            if (team.Captain.Id == userId)
                throw new Exception("Captain cannot leave the team.");

            teamRepo.RemovePlayer(team.Id, userId);
        }

        // ---------------------------------------------------------
        // KICK MEMBER
        // ---------------------------------------------------------

        public void KickMember(int captainId, int userId)
        {
            var team = teamRepo.GetTeamByUser(captainId);

            if (team == null)
                throw new Exception("Captain not in a team.");

            if (team.Captain.Id != captainId)
                throw new Exception("Only captain can kick players.");

            if (captainId == userId)
                throw new Exception("Captain cannot kick himself.");

            teamRepo.RemovePlayer(team.Id, userId);
        }
    }
}
