using LBQuiz.Models.Lobby;
using Microsoft.AspNetCore.SignalR;
using LBQuiz.Services.Interfaces;

namespace LBQuiz.Hubs
{
    public class LobbyHub : Hub
    {
        private readonly ILobbyParticipantManager _lobbyParticipantManager;
        private readonly ILobbyService _lobbyService;

        public LobbyHub(ILobbyParticipantManager lobbyParticipantManager, ILobbyService lobbyService)
        {
            _lobbyParticipantManager = lobbyParticipantManager;
            _lobbyService = lobbyService;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task JoinLobby(string joinCode, string nickname)
        {
            var lobby = await _lobbyService.GetLobbyByJoinCodeAsync(joinCode);
            if (lobby == null)
            {
                throw new HubException("Invalid join code");
            }

            var participant = new LobbyParticipant
            {
                ConnectionId = Context.ConnectionId,
                LobbyId = lobby.Id,
                Nickname = nickname,
                Score = 0
            };

            if (_lobbyParticipantManager.AddParticipant(lobby.Id, participant))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, lobby.Id.ToString());
                var participants = _lobbyParticipantManager.GetParticipants(lobby.Id);
                await Clients.Group(lobby.Id.ToString()).SendAsync("ParticipantJoined", nickname, participants);
            }
        }

        public async Task LeaveLobby()
        {
            var participant = _lobbyParticipantManager.RemoveParticipantByConnectionId(Context.ConnectionId);
            if (participant != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, participant.LobbyId.ToString());
                var participants = _lobbyParticipantManager.GetParticipants(participant.LobbyId);
                await Clients.Group(participant.LobbyId.ToString()).SendAsync("ParticipantLeft", participant.Nickname, participants);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var participant = _lobbyParticipantManager.RemoveParticipantByConnectionId(Context.ConnectionId);
            if (participant != null)
            {
                var participants = _lobbyParticipantManager.GetParticipants(participant.LobbyId);
                await Clients.Group(participant.LobbyId.ToString()).SendAsync("ParticipantLeft", participant.Nickname, participants);
            }
            await base.OnDisconnectedAsync(exception);
        }
        public async Task StartQuiz(int lobbyId, int quizId, string hostId)
        {
            await Clients.Group(lobbyId.ToString()).SendAsync("QuizLobbyStarted", quizId, lobbyId, hostId);
        }
        
        public async Task JoinLobbyAsHost(int lobbyId)
        {
            var lobby = await _lobbyService.GetLobbyByIdAsync(lobbyId);
            if (lobby == null)
            {
                throw new HubException("Lobby not found");
            }
            // Join group as host but not as a participant
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
        }

        public async Task ShowQuestion(int lobbyId, string questionText)
        {
            var lobby = await _lobbyService.GetLobbyByIdAsync(lobbyId);
            if (lobby == null)
                {
                    throw new HubException("Lobby not found");
                }
            await Clients.Group(lobbyId.ToString()).SendAsync("ShowQuestion", questionText);
        }
    }
}