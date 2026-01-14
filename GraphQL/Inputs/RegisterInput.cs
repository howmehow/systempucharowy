namespace TournamentApi.GraphQL.Inputs;

public record RegisterInput(
    string FirstName,
    string LastName,
    string Email,
    string Password
);
