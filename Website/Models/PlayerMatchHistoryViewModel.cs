using LogicLayer.Models;

namespace Website.Models;

public sealed class PlayerMatchHistoryViewModel
{
    public int UserId { get; init; }
    public IReadOnlyList<Match> Matches { get; init; } = [];
    public string EmptyMessage { get; init; } = "Tournament results will appear here.";
}
