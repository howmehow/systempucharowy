using Microsoft.EntityFrameworkCore;
using TournamentApi.Data;
using TournamentApi.Models;

namespace TournamentApi.Services;

public class BracketService : IBracketService
{
    private readonly AppDbContext _context;

    public BracketService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Bracket> GenerateBracketAsync(Tournament tournament)
    {
        var participants = tournament.Participants.ToList();
        var participantCount = participants.Count;

        if (participantCount == 0 || !IsPowerOfTwo(participantCount))
        {
            throw new InvalidOperationException("Participant count must be a power of 2");
        }

        var bracket = new Bracket
        {
            Tournament = tournament,
            TournamentId = tournament.Id
        };

        _context.Brackets.Add(bracket);
        await _context.SaveChangesAsync(); 

        var totalRounds = (int)Math.Log2(participantCount);

        var allMatches = new List<Match>();

        var round1Matches = new List<Match>();
        for (int i = 0; i < participantCount; i += 2)
        {
            var match = new Match
            {
                Bracket = bracket,
                BracketId = bracket.Id,
                Round = 1,
                Player1Id = participants[i].Id,
                Player2Id = participants[i + 1].Id
            };
            round1Matches.Add(match);
        }
        allMatches.AddRange(round1Matches);

        var previousRoundMatchCount = round1Matches.Count;
        for (int round = 2; round <= totalRounds; round++)
        {
            var matchCount = previousRoundMatchCount / 2;
            for (int i = 0; i < matchCount; i++)
            {
                var match = new Match
                {
                    Bracket = bracket,
                    BracketId = bracket.Id,
                    Round = round,
                    Player1Id = null,
                    Player2Id = null
                };
                allMatches.Add(match);
            }
            previousRoundMatchCount = matchCount;
        }

        _context.Matches.AddRange(allMatches);
        await _context.SaveChangesAsync();

        bracket.Matches = allMatches;
        return bracket;
    }

    public async Task<List<Match>> GetMatchesForRoundAsync(int bracketId, int round)
    {
        return await _context.Matches
            .Where(m => m.BracketId == bracketId && m.Round == round)
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Winner)
            .ToListAsync();
    }

    private static bool IsPowerOfTwo(int n)
    {
        return n > 0 && (n & (n - 1)) == 0;
    }
}
