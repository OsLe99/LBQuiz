namespace LBQuiz.Models.Lobby;

public class LobbyParticipant
{
    public string ConnectionId { get; set; }
    public string LobbyId { get; set; }
    public string Nickname { get; set; }
    public int? Score { get; set; }
}