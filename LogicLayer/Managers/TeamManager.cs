using LogicLayer.IRepos;
using LogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicLayer.Managers
{
    public class TeamManager
    {
        private readonly ITeamRepo teamRepo;
        private readonly INotificationRepo notificationRepo;

        public TeamManager(ITeamRepo repo, INotificationRepo notifRepo)
        {
            teamRepo = repo;
            notificationRepo = notifRepo;
        }


        public Team? GetTeamOfUser(int userId)
        {
            var team = teamRepo.GetTeamByUser(userId);
            if (team != null)
                team.Members = teamRepo.GetTeamMembers(team.Id);
            return team;
        }

        public Team? GetTeam(int id)
        {
            var t = teamRepo.GetTeamById(id);
            if (t != null)
                t.Members = teamRepo.GetTeamMembers(t.Id);
            return t;
        }

        public List<Team> GetAllTeams()
        {
            var teams = teamRepo.GetAllTeams();
            foreach (var t in teams)
                t.Members = teamRepo.GetTeamMembers(t.Id);
            return teams;
        }


        public void CreateTeam(string name, int captainId)
        {
            teamRepo.CreateTeam(name, captainId);
        }


        public void RequestJoinTeam(int teamId, int userId)
        {
            var user = teamRepo.GetUserById(userId)
                ?? throw new InvalidOperationException("User was not found.");

            if (string.IsNullOrWhiteSpace(user.SteamId) || user.SteamId == "0")
                throw new Exception("You must add your SteamID before joining a team.");

            if (teamRepo.GetTeamByUser(userId) != null)
                throw new Exception("You are already in a team.");

            var team = teamRepo.GetTeamById(teamId)
                ?? throw new InvalidOperationException("Team was not found.");
            var members = teamRepo.GetTeamMembers(teamId);

            if (members.Count >= 5)
                throw new Exception("This team is already full (maximum 5 players).");

            if (teamRepo.GetRequestsForUser(userId).Any(r => r.TeamId == teamId))
                throw new Exception("Join request already exists.");

            teamRepo.CreateJoinRequest(teamId, userId);

            if (team.Captain.Id is not int captainId)
                throw new InvalidOperationException("Team captain was not found.");

            notificationRepo.Create(
                captainId,
                $"{user.Username} wants to join your team '{team.Name}'.",
                $"/Teams/Details?id={team.Id}"
            );
        }



        public List<TeamJoinRequest> GetJoinRequests(int teamId)
        {
            return teamRepo.GetRequestsForTeam(teamId);
        }

        public List<TeamJoinRequest> GetRequestsForUser(int userId)
        {
            return teamRepo.GetRequestsForUser(userId);
        }


        public void ApproveRequest(int requestId, int captainId)
        {
            var captainTeam = teamRepo.GetTeamByUser(captainId);
            if (captainTeam == null)
                throw new Exception("Captain is not in a team.");

            if (captainTeam.Captain.Id != captainId)
                throw new Exception("Only the captain can approve requests.");

            var req = teamRepo.GetRequestsForTeam(captainTeam.Id)
                              .FirstOrDefault(r => r.Id == requestId);

            if (req == null)
                throw new Exception("Request not found.");

            var user = teamRepo.GetUserById(req.UserId)
                ?? throw new InvalidOperationException("User was not found.");
            if (string.IsNullOrWhiteSpace(user.SteamId))
                throw new Exception("User must add SteamID before joining team.");

            teamRepo.AddPlayerToTeam(
                captainTeam.Id,
                req.UserId,
                "Member",
                "Approved"
            );

            teamRepo.DeleteJoinRequest(requestId);

            notificationRepo.Create(
                req.UserId,
                $"Your request to join team '{captainTeam.Name}' was accepted.",
                $"/Teams/Details?id={captainTeam.Id}"
            );
        }


        public void RejectRequest(int requestId, int captainId)
        {
            var captainTeam = teamRepo.GetTeamByUser(captainId);
            if (captainTeam == null)
                throw new Exception("Captain is not in a team.");

            if (captainTeam.Captain.Id != captainId)
                throw new Exception("Only the captain can reject requests.");

            var req = teamRepo.GetRequestsForTeam(captainTeam.Id)
                              .FirstOrDefault(r => r.Id == requestId);

            if (req == null)
                throw new Exception("Request not found.");

            teamRepo.DeleteJoinRequest(requestId);

            notificationRepo.Create(
                req.UserId,
                $"Your request to join team '{captainTeam.Name}' was rejected."
            );
        }


        public void LeaveTeam(int userId)
        {
            var team = teamRepo.GetTeamByUser(userId);
            if (team == null)
                throw new Exception("User is not in a team.");

            if (team.Captain.Id == userId)
            {
                teamRepo.DeleteTeam(team.Id);
                return;
            }

            teamRepo.RemovePlayer(team.Id, userId);
        }

        public void KickMember(int captainId, int userId)
        {
            var team = teamRepo.GetTeamByUser(captainId);
            if (team == null)
                throw new Exception("Captain not in team.");

            if (team.Captain.Id != captainId)
                throw new Exception("Only the captain can kick players.");

            if (captainId == userId)
                throw new Exception("Captain cannot kick himself.");

            teamRepo.RemovePlayer(team.Id, userId);
        }
    }
}
