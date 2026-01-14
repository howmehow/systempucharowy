namespace TournamentApi.GraphQL.Inputs;

public record LoginInput(
    string Email,
    string Password
);
