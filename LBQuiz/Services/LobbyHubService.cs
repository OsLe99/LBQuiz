using LBQuiz.Hubs;
using LBQuiz.Services.Interfaces;
using Microsoft.Build.Framework;
using Microsoft.AspNetCore.SignalR;

namespace LBQuiz.Services
{
    public class LobbyHubService : ILobbyHubService
    {
        private readonly IHubContext<LobbyHub> _hubContext;
        public LobbyHubService(IHubContext<LobbyHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SubmitAnswerAsync(string answer, int questionId)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveSubmittedAnswer", answer, questionId);
        }

    }
}
