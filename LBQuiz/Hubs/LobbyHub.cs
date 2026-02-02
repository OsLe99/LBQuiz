using Microsoft.AspNetCore.SignalR;

namespace LBQuiz.Hubs
{
    public class LobbyHub : Hub
    {
        //Lägg till routing till Lobbyhub i Program.cs
        public string LobbyHubConnectionId()
        {
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"LOBBY HUB CONNECTED: {connectionId}");
            return connectionId;
        }
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("LOBBY HUB CONNECTED");
            await base.OnConnectedAsync();
            var hub = new LobbyHub();
        }
    }
}
