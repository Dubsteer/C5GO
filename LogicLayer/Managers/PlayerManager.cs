using LogicLayer.IRepos;
using LogicLayer.Models;

namespace LogicLayer.Managers
{
    public class PlayerManager
    {
        private readonly IPlayerRepo repo;
        private readonly ITeamRepo teamRepo;
        private readonly ITournamentRepo tournamentRepo;

        public PlayerManager(
            IPlayerRepo repo,
            ITeamRepo teamRepo,
            ITournamentRepo tournamentRepo)
        {
            this.repo = repo;
            this.teamRepo = teamRepo;
            this.tournamentRepo = tournamentRepo;
        }

        public Player? GetPlayer(User u) => repo.GetPlayer(u);

        public List<Player> GetAllPlayers() => repo.GetAllPlayers();

        public void RemovePlayerRole(int userId)
        {
            if (teamRepo.GetTeamByUser(userId) != null)
                throw new InvalidOperationException("The player must leave their team before removing the SteamID.");

            if (tournamentRepo.HasActivePlayerRegistration(userId))
                throw new InvalidOperationException("The player must leave active tournaments before removing the SteamID.");

            if (!repo.DeletePlayerRole(userId))
                throw new InvalidOperationException("The selected user does not have a player profile.");
        }
    }
}
