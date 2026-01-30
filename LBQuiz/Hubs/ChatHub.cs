using Microsoft.AspNetCore.SignalR;

namespace LBQuiz.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("CHAT HUB CONNECTED");
            await base.OnConnectedAsync();
        }
    }
}
