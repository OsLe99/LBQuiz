using LBQuiz.Models.Lobby;
using Microsoft.AspNetCore.Components;

namespace LBQuiz.Services.Interfaces
{
    public interface ILobbyHubConnection
    {
        Task InitializeAsync(NavigationManager navigation, string? userId = null);
        Task JoinLobbyAsync(string joinCode, string nickname);
        Task JoinLobbyAsHostAsync(int lobbyId);
        Task LeaveLobbyAsync();
        Task StartQuizAsync(int lobbyId, int quizId, string hostId);
        List<LobbyParticipant> Participants { get; }
        event Func<Task>? OnParticipantsChanged;
    }
}
