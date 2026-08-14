using LogicLayer.IRepos;
using LogicLayer.Models;

namespace Unit_Tests.MockRepos;

public class MockTeamRepo : ITeamRepo
{
    private readonly List<User> users;
    private readonly Dictionary<int, List<User>> membersByTeam = [];

    public MockTeamRepo(IEnumerable<User> users)
    {
        this.users = users.ToList();
    }

    public List<Team> Teams { get; } = [];
    public List<TeamJoinRequest> Requests { get; } = [];

    public void SeedTeam(Team team, params User[] members)
    {
        Teams.Add(team);
        membersByTeam[team.Id] = members.ToList();
    }

    public Team? GetTeamById(int id) => Teams.FirstOrDefault(team => team.Id == id);

    public Team? GetTeamByUser(int userId) => Teams.FirstOrDefault(team =>
        membersByTeam.TryGetValue(team.Id, out var members) &&
        members.Any(user => user.Id == userId));

    public List<Team> GetAllTeams() => Teams.ToList();

    public List<User> GetTeamMembers(int teamId) =>
        membersByTeam.TryGetValue(teamId, out var members)
            ? members.ToList()
            : [];

    public void CreateTeam(string name, int captainId)
    {
        var captain = GetUserById(captainId)
            ?? throw new InvalidOperationException("Captain was not found.");
        var team = new Team(Teams.Count + 1, name, captain);
        SeedTeam(team, captain);
    }

    public void AddPlayerToTeam(int teamId, int userId, string role, string status)
    {
        var user = GetUserById(userId)
            ?? throw new InvalidOperationException("User was not found.");
        if (!membersByTeam.TryGetValue(teamId, out var members))
            membersByTeam[teamId] = members = [];
        members.Add(user);
    }

    public void UpdatePlayerStatus(int teamId, int userId, string newStatus)
    {
    }

    public void RemovePlayer(int teamId, int userId)
    {
        if (membersByTeam.TryGetValue(teamId, out var members))
            members.RemoveAll(user => user.Id == userId);
    }

    public void CreateJoinRequest(int teamId, int userId)
    {
        Requests.Add(new TeamJoinRequest
        {
            Id = Requests.Count + 1,
            TeamId = teamId,
            UserId = userId,
            RequestedAt = DateTime.UtcNow
        });
    }

    public void DeleteJoinRequest(int requestId) =>
        Requests.RemoveAll(request => request.Id == requestId);

    public List<TeamJoinRequest> GetRequestsForTeam(int teamId) =>
        Requests.Where(request => request.TeamId == teamId).ToList();

    public List<TeamJoinRequest> GetRequestsForUser(int userId) =>
        Requests.Where(request => request.UserId == userId).ToList();

    public User? GetUserById(int userId) => users.FirstOrDefault(user => user.Id == userId);

    public void DeleteTeam(int id)
    {
        Teams.RemoveAll(team => team.Id == id);
        membersByTeam.Remove(id);
        Requests.RemoveAll(request => request.TeamId == id);
    }
}
