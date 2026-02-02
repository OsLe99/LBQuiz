using LBQuiz.Models.Lobby;

namespace LBQuiz.Services.Interfaces
{
    public interface ILobbyParticipantManager
    {
        bool AddParticipant(int lobbyId, LobbyParticipant participant);
        List<LobbyParticipant> GetParticipants(int lobbyId);
        List<LobbyParticipant> GetLeaderboard(int lobbyId);
        LobbyParticipant? RemoveParticipantByConnectionId(string connectionId);
    }
}
