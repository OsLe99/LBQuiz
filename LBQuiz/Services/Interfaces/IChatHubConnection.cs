using Microsoft.AspNetCore.Components;

namespace LBQuiz.Services.Interfaces
{
    public interface IChatHubConnection
    {
        Task InitializeAsync(NavigationManager navigation, string? userId = null);
        event Func<string, string, Task>? OnMessageRecived;
        Task SendMessage(string userName, string message);
    }
}
