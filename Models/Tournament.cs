namespace TournamentApi.Models;

public class Tournament
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime StartDate { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.Pending;

    // Navigation properties
    public ICollection<User> Participants { get; set; } = new List<User>();
    public int? BracketId { get; set; }
    public Bracket? Bracket { get; set; }
}
