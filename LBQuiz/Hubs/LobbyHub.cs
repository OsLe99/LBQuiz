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
        public async Task StartQuiz(string joinCode, int lobbyId, string hostId)
        {
            await Clients.Group(lobbyId.ToString()).SendAsync("QuizLobbyStarted", joinCode, lobbyId, hostId);
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

        public async Task ReceiveSubmittedAnswer(string answer, string lobbyId, int quizId)
        {
            var participant = _lobbyParticipantManager.GetLobbyParticipant(Context.ConnectionId);
            if(participant != null)
            {
                // Send a consistent server-to-client event name and payload
                await Clients.Group(participant.LobbyId.ToString()).SendAsync("AnswerReceived", answer, quizId, participant);
                Console.WriteLine("LobbyHub : " + participant.LobbyId);
            }
        }

        public async Task CheckParticipantAnswer(int currentQuestion, string currentCorrectAnswer, int points)
        {
            var connectionId = Context.ConnectionId;
            if (await _lobbyParticipantManager.CheckAnswer(currentCorrectAnswer, currentQuestion, connectionId))
            {
                // Answer is correct, you can implement additional logic here if needed
                var participant = _lobbyParticipantManager.GetLobbyParticipant(connectionId);
                participant.Score += points;
            }
        }

        //H�r ska logiken f�r att r�kna ut po�ngst�llningen in
        public async Task CalculateScoreBoard(Models.QuestionOpen Question, string answer)
        {
            var participant = _lobbyParticipantManager.GetLobbyParticipant(Context.ConnectionId);
            if (participant == null) return;
            if(Question.CorrectAnswer.Equals(answer, StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(Question.CorrectAnswer, answer, StringComparison.OrdinalIgnoreCase))
                {
                    participant.Score += Question.Points;
                }
                
                await Clients.Group(participant.LobbyId.ToString()).SendAsync("ScoreBoardCalculated", Question, answer, participant);
            }

            var participants = _lobbyParticipantManager.GetParticipants(participant.LobbyId);

            await Clients.Group(participant.LobbyId.ToString()).SendAsync("ScoreBoardUpdated", participants);
            
        }

        public async Task GoToNextQuestionAsync(int questionIndex, string lobbyId)
        {
            await Clients.Group(lobbyId).SendAsync("GoToNextQuestion", questionIndex);
        }

        public async Task GoToPreviousQuestionAsync(int questionIndex, string lobbyId)
        {
            await Clients.Group(lobbyId).SendAsync("GoToPreviousQuestion", questionIndex);
        }

        public async Task GoToResultsAsync(bool showResults, string lobbyId, List<LobbyParticipant> scoreBoard)
        {
            await Clients.Group(lobbyId).SendAsync("GoToResults", showResults, scoreBoard);
        }

        public async Task EndQuiz(string lobbyId)
        {
            await _lobbyService.EndQuizAsync(lobbyId);
            await Clients.Group(lobbyId).SendAsync("QuizEnded");

            var participants = _lobbyParticipantManager.GetParticipants(int.Parse(lobbyId));
            foreach (var participant in participants.ToList())
            {
                await Groups.RemoveFromGroupAsync(participant.ConnectionId, participant.LobbyId.ToString());
                _lobbyParticipantManager.RemoveParticipantByConnectionId(participant.ConnectionId);
            }
        }
        public async Task ReConnectToRoom(string lobbyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
        }

    }
}