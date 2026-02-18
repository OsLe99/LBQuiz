using LBQuiz.Data;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LBQuiz.Hubs
{
    public class ChatHub : Hub
    {
        //Här har vi möjlighet att kalla på Clients som i sin tur kan anropa Metoder i ChatHubConnetcion
        private ApplicationDbContext _dbContext;
        private string? GetUserId() => Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public ChatHub(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task SendMessages(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
