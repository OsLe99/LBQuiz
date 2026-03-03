using LBQuiz.Data;
using LBQuiz.Models.Helpers;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LBQuiz.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task JoinLobbyChat(string lobbyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{lobbyId}");
        }

        public async Task SendMessages(ChatMessage playMessage)
        {
            if (!string.IsNullOrEmpty(playMessage.LobbyId))
            {
                await Clients.Group($"chat-{playMessage.LobbyId}").SendAsync("ReceiveMessage", playMessage);
            }
        }
    }
}
