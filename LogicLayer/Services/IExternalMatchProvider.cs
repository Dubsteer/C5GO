using LogicLayer.Dtos;

namespace LogicLayer.Services
{
    public interface IExternalMatchProvider
    {
        Task<List<ExternalMatchDto>> GetTodayMatchesAsync();

        Task<List<ExternalMatchDto>> GetRecentMatchesAsync(int limit = 20);

        Task<ExternalMatchDetailsDto?> GetMatchDetailsAsync(
            string matchId,
            bool preferPast = false);
    }
}
