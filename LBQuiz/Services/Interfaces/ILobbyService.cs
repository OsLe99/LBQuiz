using LBQuiz.Models.Lobby;

namespace LBQuiz.Services.Interfaces;

public interface ILobbyService
{
    Task<QuizLobby> CreateLobbyAsync(int quizId);
    Task<QuizLobby?> GetLobbyByJoinCodeAsync(string joinCode);
    Task<QuizLobby?> GetLobbyByIdAsync(int lobbyId);
}
