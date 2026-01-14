using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using TournamentApi.Data;
using TournamentApi.Models;
using TournamentApi.Services;

namespace TournamentApi.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class MatchMutations
{
    public async Task<Match> PlayMatch(int matchId, int winnerId, [Service] IMatchService matchService, [Service] AppDbContext context)
    {
        var match = await matchService.PlayMatchAsync(matchId, winnerId);

        // Reload with all relationships
        return await context.Matches
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Winner)
            .Include(m => m.Bracket)
            .FirstAsync(m => m.Id == match.Id);
    }
}
