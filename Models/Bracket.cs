namespace TournamentApi.Models;

public class Bracket
{
    public int Id { get; set; }
    public int TournamentId { get; set; }

    public required Tournament Tournament { get; set; }
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}
