using TournamentApi.Models;

namespace TournamentApi.GraphQL.Types;

public class AuthPayload
{
    public required string Token { get; set; }
    public required User User { get; set; }
}
