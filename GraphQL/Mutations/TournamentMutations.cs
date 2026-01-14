using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using TournamentApi.Data;
using TournamentApi.GraphQL.Inputs;
using TournamentApi.Models;
using TournamentApi.Services;

namespace TournamentApi.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class TournamentMutations
{
    public async Task<Tournament> CreateTournament(CreateTournamentInput input, [Service] AppDbContext context)
    {
        var tournament = new Tournament
        {
            Name = input.Name,
            StartDate = input.StartDate,
            Status = TournamentStatus.Pending
        };

        context.Tournaments.Add(tournament);
        await context.SaveChangesAsync();

        return tournament;
    }

    public async Task<Tournament> AddParticipant(int tournamentId, int userId, [Service] ITournamentService tournamentService)
    {
        return await tournamentService.AddParticipantAsync(tournamentId, userId);
    }

    public async Task<Tournament> StartTournament(int tournamentId, [Service] ITournamentService tournamentService, [Service] AppDbContext context)
    {
        var tournament = await tournamentService.StartTournamentAsync(tournamentId);

        return await context.Tournaments
            .Include(t => t.Participants)
            .Include(t => t.Bracket)
                .ThenInclude(b => b!.Matches)
                    .ThenInclude(m => m.Player1)
            .Include(t => t.Bracket)
                .ThenInclude(b => b!.Matches)
                    .ThenInclude(m => m.Player2)
            .FirstAsync(t => t.Id == tournament.Id);
    }

    public async Task<Tournament> FinishTournament(int tournamentId, [Service] ITournamentService tournamentService)
    {
        return await tournamentService.FinishTournamentAsync(tournamentId);
    }
}
