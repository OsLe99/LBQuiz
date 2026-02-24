using LBQuiz.Models.Lobby;

namespace LBQuiz.Services.Interfaces
{
    public interface ILobbyParticipantManager
    {
        bool AddParticipant(int lobbyId, LobbyParticipant participant);
        List<LobbyParticipant> GetParticipants(int lobbyId);
        List<LobbyParticipant> GetLeaderboard(int lobbyId);
        LobbyParticipant? RemoveParticipantByConnectionId(string connectionId);
        Task SubmitParticipantAnswer(string connectionId, string answer, int questionId);
        Task<bool> CheckAnswer(string correctAnswer, int questionId, string connectionId);
        LobbyParticipant GetLobbyParticipant(string connectionId);
        bool UpdateParticipantConnectionId(string oldConnectionId, string newConnectionId);
    }
}
