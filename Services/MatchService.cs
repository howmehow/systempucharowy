using Microsoft.EntityFrameworkCore;
using TournamentApi.Data;
using TournamentApi.Models;

namespace TournamentApi.Services;

public class MatchService : IMatchService
{
    private readonly AppDbContext _context;

    public MatchService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Match> PlayMatchAsync(int matchId, int winnerId)
    {
        var match = await _context.Matches
            .Include(m => m.Bracket)
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .FirstOrDefaultAsync(m => m.Id == matchId);

        if (match == null)
        {
            throw new InvalidOperationException("Match not found");
        }

        if (match.WinnerId != null)
        {
            throw new InvalidOperationException("Match has already been played");
        }

        // Validate that winner is one of the players
        if (winnerId != match.Player1Id && winnerId != match.Player2Id)
        {
            throw new InvalidOperationException("Winner must be one of the players in the match");
        }

        match.WinnerId = winnerId;
        await _context.SaveChangesAsync();

        // Advance winner to next round if not final round
        await AdvanceWinnerToNextRound(match, winnerId);

        return match;
    }

    private async Task AdvanceWinnerToNextRound(Match currentMatch, int winnerId)
    {
        var nextRound = currentMatch.Round + 1;

        // Find matches in the next round for this bracket
        var nextRoundMatches = await _context.Matches
            .Where(m => m.BracketId == currentMatch.BracketId && m.Round == nextRound)
            .OrderBy(m => m.Id)
            .ToListAsync();

        if (nextRoundMatches.Count == 0)
        {
            // This was the final round
            return;
        }

        // Find current match's position in its round
        var currentRoundMatches = await _context.Matches
            .Where(m => m.BracketId == currentMatch.BracketId && m.Round == currentMatch.Round)
            .OrderBy(m => m.Id)
            .ToListAsync();

        var currentMatchIndex = currentRoundMatches.IndexOf(currentMatch);
        var nextMatchIndex = currentMatchIndex / 2;

        if (nextMatchIndex < nextRoundMatches.Count)
        {
            var nextMatch = nextRoundMatches[nextMatchIndex];

            // Assign winner to next match (alternating between Player1 and Player2)
            if (currentMatchIndex % 2 == 0)
            {
                nextMatch.Player1Id = winnerId;
            }
            else
            {
                nextMatch.Player2Id = winnerId;
            }

            await _context.SaveChangesAsync();
        }
    }
}
