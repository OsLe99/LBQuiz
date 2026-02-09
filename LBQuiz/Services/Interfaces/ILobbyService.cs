using LBQuiz.Models.Lobby;

namespace LBQuiz.Services.Interfaces;

public interface ILobbyService
{
    Task<QuizLobby> CreateLobbyAsync(int quizId, string hostId);
    Task<QuizLobby?> GetLobbyByJoinCodeAsync(string joinCode);
    Task<QuizLobby?> GetLobbyByIdAsync(int lobbyId);
    Task EndQuizAsync(int lobbyId);
}
