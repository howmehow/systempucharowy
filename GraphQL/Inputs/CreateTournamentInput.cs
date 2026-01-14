namespace TournamentApi.GraphQL.Inputs;

public record CreateTournamentInput(
    string Name,
    DateTime StartDate
);
