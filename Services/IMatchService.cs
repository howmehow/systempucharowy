using TournamentApi.Models;

namespace TournamentApi.Services;

public interface IMatchService
{
    Task<Match> PlayMatchAsync(int matchId, int winnerId);
}
