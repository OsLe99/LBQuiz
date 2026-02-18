using System.ComponentModel.DataAnnotations;

namespace LBQuiz.Models.Lobby;

public class LobbyParticipant
{
    public string ConnectionId { get; set; }
    public int LobbyId { get; set; }
    [Required(ErrorMessage = "Nickname is required")]
    public required string Nickname { get; set; }
    public int? Score { get; set; }
}