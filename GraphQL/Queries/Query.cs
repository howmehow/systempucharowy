using System.Security.Claims;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using TournamentApi.Data;
using TournamentApi.Models;

namespace TournamentApi.GraphQL.Queries;

public class Query
{
    public async Task<List<User>> GetUsers([Service] AppDbContext context)
    {
        return await context.Users.ToListAsync();
    }

    public async Task<List<Tournament>> GetTournaments([Service] AppDbContext context)
    {
        return await context.Tournaments
            .Include(t => t.Participants)
            .Include(t => t.Bracket)
            .ToListAsync();
    }

    public async Task<Tournament?> GetTournament(int id, [Service] AppDbContext context)
    {
        return await context.Tournaments
            .Include(t => t.Participants)
            .Include(t => t.Bracket)
                .ThenInclude(b => b!.Matches)
                    .ThenInclude(m => m.Player1)
            .Include(t => t.Bracket)
                .ThenInclude(b => b!.Matches)
                    .ThenInclude(m => m.Player2)
            .Include(t => t.Bracket)
                .ThenInclude(b => b!.Matches)
                    .ThenInclude(m => m.Winner)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Bracket?> GetBracket(int id, [Service] AppDbContext context)
    {
        return await context.Brackets
            .Include(b => b.Tournament)
            .Include(b => b.Matches)
                .ThenInclude(m => m.Player1)
            .Include(b => b.Matches)
                .ThenInclude(m => m.Player2)
            .Include(b => b.Matches)
                .ThenInclude(m => m.Winner)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    [Authorize]
    public async Task<List<Match>> GetMyMatches([Service] IHttpContextAccessor httpContextAccessor, [Service] AppDbContext context)
    {
        var httpContext = httpContextAccessor.HttpContext ?? throw new UnauthorizedAccessException("No HTTP context available");
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        return await context.Matches
            .Where(m => m.Player1Id == userId || m.Player2Id == userId)
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Winner)
            .Include(m => m.Bracket)
                .ThenInclude(b => b.Tournament)
            .ToListAsync();
    }
}
