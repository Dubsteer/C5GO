using LogicLayer.Dtos;

namespace LogicLayer.Services
{
    public interface IExternalMatchProvider
    {
        Task<List<ExternalMatchDto>> GetTodayMatchesAsync();

        Task<ExternalMatchDetailsDto?> GetMatchDetailsAsync(string matchId);
    }
}