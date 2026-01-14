namespace TournamentApi.Models;

public class Match
{
    public int Id { get; set; }
    public int BracketId { get; set; }
    public int Round { get; set; }

    public int? Player1Id { get; set; }
    public int? Player2Id { get; set; }
    public int? WinnerId { get; set; }

    public required Bracket Bracket { get; set; }
    public User? Player1 { get; set; }
    public User? Player2 { get; set; }
    public User? Winner { get; set; }
}
