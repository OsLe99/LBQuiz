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

        public async Task SendMessages(ChatMessage playMessage)
        {
            await Clients.All.SendAsync("ReceiveMessage", playMessage);
        }
    }
}
