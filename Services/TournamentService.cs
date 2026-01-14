using Microsoft.EntityFrameworkCore;
using TournamentApi.Data;
using TournamentApi.Models;

namespace TournamentApi.Services;

public class TournamentService : ITournamentService
{
    private readonly AppDbContext _context;
    private readonly IBracketService _bracketService;

    public TournamentService(AppDbContext context, IBracketService bracketService)
    {
        _context = context;
        _bracketService = bracketService;
    }

    public async Task<Tournament> AddParticipantAsync(int tournamentId, int userId)
    {
        var tournament = await _context.Tournaments
            .Include(t => t.Participants)
            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        if (tournament == null)
        {
            throw new InvalidOperationException("Tournament not found");
        }

        if (tournament.Status != TournamentStatus.Pending)
        {
            throw new InvalidOperationException("Cannot add participants to a tournament that has already started");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        if (tournament.Participants.Any(p => p.Id == userId))
        {
            throw new InvalidOperationException("User is already a participant");
        }

        tournament.Participants.Add(user);
        await _context.SaveChangesAsync();

        return tournament;
    }

    public async Task<Tournament> StartTournamentAsync(int tournamentId)
    {
        var tournament = await _context.Tournaments
            .Include(t => t.Participants)
            .Include(t => t.Bracket)
            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        if (tournament == null)
        {
            throw new InvalidOperationException("Tournament not found");
        }

        if (tournament.Status != TournamentStatus.Pending)
        {
            throw new InvalidOperationException("Tournament has already started");
        }

        var participantCount = tournament.Participants.Count;

        if (participantCount < 2 || !IsPowerOfTwo(participantCount))
        {
            throw new InvalidOperationException($"Tournament must have a power of 2 participants (2, 4, 8, 16, etc.). Current count: {participantCount}");
        }

        var bracket = await _bracketService.GenerateBracketAsync(tournament);
        tournament.Bracket = bracket;
        tournament.BracketId = bracket.Id;
        tournament.Status = TournamentStatus.InProgress;

        await _context.SaveChangesAsync();

        return tournament;
    }

    public async Task<Tournament> FinishTournamentAsync(int tournamentId)
    {
        var tournament = await _context.Tournaments.FindAsync(tournamentId);

        if (tournament == null)
        {
            throw new InvalidOperationException("Tournament not found");
        }

        if (tournament.Status != TournamentStatus.InProgress)
        {
            throw new InvalidOperationException("Tournament is not in progress");
        }

        tournament.Status = TournamentStatus.Finished;
        await _context.SaveChangesAsync();

        return tournament;
    }

    private static bool IsPowerOfTwo(int n)
    {
        return n > 0 && (n & (n - 1)) == 0;
    }
}
