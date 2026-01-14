using TournamentApi.Models;

namespace TournamentApi.Services;

public interface IAuthService
{
    Task<(string Token, User User)> RegisterAsync(string firstName, string lastName, string email, string password);
    Task<(string Token, User User)?> LoginAsync(string email, string password);
}
