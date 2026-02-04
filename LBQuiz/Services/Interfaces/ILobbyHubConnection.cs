using LBQuiz.Models.Lobby;
using Microsoft.AspNetCore.Components;

namespace LBQuiz.Services.Interfaces
{
    public interface ILobbyHubConnection
    {
        Task InitializeAsync(NavigationManager navigation);
        Task JoinLobbyAsync(string joinCode, string nickname);
        Task LeaveLobbyAsync();
        Task StartQuizAsync(int lobbyId, int quizId);
        List<LobbyParticipant> Participants { get; }
        event Func<Task>? OnParticipantsChanged;
        event Func<string, Models.Lobby.LobbyParticipant, Task>? OnAnswerRecieved;
        Task SubmitAnswer(string lobbyId, string answer, int quizId);
        

    }
}
