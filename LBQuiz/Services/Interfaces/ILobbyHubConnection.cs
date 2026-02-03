using LBQuiz.Models.Lobby;
using Microsoft.AspNetCore.Components;

namespace LBQuiz.Services.Interfaces
{
    public interface ILobbyHubConnection
    {
        Task InitializeAsync(NavigationManager navigation);
        Task JoinLobbyAsync(string joinCode, string nickname);
        Task LeaveLobbyAsync();
        List<LobbyParticipant> Participants { get; }
        event Func<Task>? OnParticipantsChanged;
    }
}
