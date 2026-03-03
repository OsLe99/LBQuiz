using LBQuiz.Models.Helpers;
using Microsoft.AspNetCore.Components;

namespace LBQuiz.Services.Interfaces
{
    public interface IChatHubConnection
    {
        Task InitializeAsync(NavigationManager navigation, string? userId = null);
        Task JoinLobbyChatAsync(string lobbyId);
        event Func<ChatMessage, Task>? OnMessageRecived;
        Task SendMessage(ChatMessage playMessage);
    }
}
