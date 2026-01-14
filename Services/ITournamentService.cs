using TournamentApi.Models;

namespace TournamentApi.Services;

public interface ITournamentService
{
    Task<Tournament> AddParticipantAsync(int tournamentId, int userId);
    Task<Tournament> StartTournamentAsync(int tournamentId);
    Task<Tournament> FinishTournamentAsync(int tournamentId);
}
