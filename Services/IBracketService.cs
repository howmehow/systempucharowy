using TournamentApi.Models;

namespace TournamentApi.Services;

public interface IBracketService
{
    Task<Bracket> GenerateBracketAsync(Tournament tournament);
    Task<List<Match>> GetMatchesForRoundAsync(int bracketId, int round);
}
