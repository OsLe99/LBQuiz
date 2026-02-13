using LBQuiz.Models;
using LBQuiz.Models.Helpers;
using LBQuiz.Models.Lobby;
using LBQuiz.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LBQuiz.Hubs
{
    public class LobbyHub : Hub
    {
        private readonly ILobbyParticipantManager _lobbyParticipantManager;
        private readonly ILobbyService _lobbyService;
        private string? GetUserId() => Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public LobbyHub(ILobbyParticipantManager lobbyParticipantManager, ILobbyService lobbyService)
        {
            _lobbyParticipantManager = lobbyParticipantManager;
            _lobbyService = lobbyService;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        private async Task EnsureIsHostAsync(int lobbyId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                throw new HubException("Invalid host id");
            }
            var lobby = await _lobbyService.GetLobbyByIdAsync(lobbyId);
            if (lobby?.QuizHostId != userId)
            {
                throw new HubException("Only for hosts.");
            }
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
        public async Task StartQuiz(int lobbyId, int quizId)
        {
            await EnsureIsHostAsync(lobbyId);
            var hostId = GetUserId();
            await Clients.Group(lobbyId.ToString()).SendAsync("QuizLobbyStarted", quizId, lobbyId, hostId);
        }
        
        public async Task JoinLobbyAsHost(int lobbyId)
        {
            // var lobby = await _lobbyService.GetLobbyByIdAsync(lobbyId);
            // if (lobby == null)
            // {
            //     throw new HubException("Lobby not found");
            // }
            // Join group as host but not as a participant
            await EnsureIsHostAsync(lobbyId);
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

        public async Task GoToNextQuestionAsync(int questionIndex, int lobbyId)
        {
            await EnsureIsHostAsync(lobbyId);
            await Clients.Group(lobbyId.ToString()).SendAsync("GoToNextQuestion", questionIndex);
        }

        public async Task GoToPreviousQuestionAsync(int questionIndex, int lobbyId)
        {
            await EnsureIsHostAsync(lobbyId);
            await Clients.Group(lobbyId.ToString()).SendAsync("GoToPreviousQuestion", questionIndex);
        }

        public async Task GoToResultsAsync(bool showResults, int lobbyId, List<LobbyParticipant> scoreBoard)
        {
            await EnsureIsHostAsync(lobbyId);
            await Clients.Group(lobbyId.ToString()).SendAsync("GoToResults", showResults, scoreBoard);
        }
        
        public async Task EndQuiz(int lobbyId)
        {
            await EnsureIsHostAsync(lobbyId);
            await _lobbyService.EndQuizAsync(lobbyId);
            await Clients.Group(lobbyId.ToString()).SendAsync("QuizEnded");

            var participants = _lobbyParticipantManager.GetParticipants(lobbyId);
            foreach (var participant in participants.ToList())
            {
                await Groups.RemoveFromGroupAsync(participant.ConnectionId, participant.LobbyId.ToString());
                _lobbyParticipantManager.RemoveParticipantByConnectionId(participant.ConnectionId);
            }
        }
        public async Task SubmitSliderAnswer(int lobbyId, int sliderValue, int quizId, string questionText)
        {
            var participant = _lobbyParticipantManager.GetLobbyParticipant(Context.ConnectionId);
            if (participant != null)
            {
                // Send a consistent server-to-client event name and payload
                await Clients.Group(participant.LobbyId.ToString()).SendAsync("SliderAnswerSubmit", sliderValue, quizId, participant, questionText);
            }
        }

        public async Task SubmitMultipleAnswers(int lobbyid, int quizId, List<MultipleOptions> options, List<MultipleOptions> participantAnswers)
        {
            var participant = _lobbyParticipantManager.GetLobbyParticipant(Context.ConnectionId);
            if (participant != null)
            {
                await Clients.Group(participant.LobbyId.ToString()).SendAsync("MultipleAnswersSubmits", participant, quizId, options, participantAnswers);
            }
        }


    }
}