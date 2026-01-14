namespace TournamentApi.Models;

public class User
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    // Navigation properties
    public ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
    public ICollection<Match> MatchesAsPlayer1 { get; set; } = new List<Match>();
    public ICollection<Match> MatchesAsPlayer2 { get; set; } = new List<Match>();
    public ICollection<Match> MatchesWon { get; set; } = new List<Match>();
}
