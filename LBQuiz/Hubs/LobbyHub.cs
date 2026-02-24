using LBQuiz.Models;
using LBQuiz.Models.Helpers;
using LBQuiz.Models.Lobby;
using LBQuiz.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using LBQuiz.Data;
using Microsoft.EntityFrameworkCore;

namespace LBQuiz.Hubs
{
    public class LobbyHub : Hub
    {
        private readonly ILobbyParticipantManager _lobbyParticipantManager;
        private readonly ILobbyService _lobbyService;
        private readonly IQuestionScoringService _scoringService;
        private readonly ApplicationDbContext _dbContext;
        private string? GetUserId() => Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public LobbyHub(ILobbyParticipantManager lobbyParticipantManager, ILobbyService lobbyService, IQuestionScoringService scoringService, ApplicationDbContext dbContext)
        {
            _lobbyParticipantManager = lobbyParticipantManager;
            _lobbyService = lobbyService;
            _scoringService = scoringService;
            _dbContext = dbContext;
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

            var existingParticipants = _lobbyParticipantManager.GetParticipants(lobby.Id);
            if (existingParticipants.Any(p => p.Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase)))
            {
                throw new HubException("A player with that nickname is already in the lobby.");
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
            // Join group as host but not as a participant
            await EnsureIsHostAsync(lobbyId);
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
        }

        public async Task RejoinLobby(int lobbyId, string nickname)
        {
            var lobby = await _lobbyService.GetLobbyByIdAsync(lobbyId);
            if (lobby == null)
            {
                throw new HubException("Lobby not found");
            }

            // Check if there's already a participant with this nickname
            var existingParticipants = _lobbyParticipantManager.GetParticipants(lobbyId);
            var existing = existingParticipants.FirstOrDefault(p => p.Nickname == nickname);

            if (existing != null)
            {
                if (existing.ConnectionId == Context.ConnectionId)
                {
                    // Already connected, just ensure group membership
                    await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
                    return;
                }

                // Update to the new connection ID
                _lobbyParticipantManager.UpdateParticipantConnectionId(existing.ConnectionId, Context.ConnectionId);
                await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
                var participants = _lobbyParticipantManager.GetParticipants(lobbyId);
                await Clients.Group(lobbyId.ToString()).SendAsync("ParticipantJoined", nickname, participants);
                return;
            }

            // Re add as a participant 
            var participant = new LobbyParticipant
            {
                ConnectionId = Context.ConnectionId,
                LobbyId = lobbyId,
                Nickname = nickname,
                Score = 0
            };

            if (_lobbyParticipantManager.AddParticipant(lobbyId, participant))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
                var participants = _lobbyParticipantManager.GetParticipants(lobbyId);
                await Clients.Group(lobbyId.ToString()).SendAsync("ParticipantJoined", nickname, participants);
            }
        }

        public async Task RejoinLobbyAsHost(int lobbyId)
        {
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
        public async Task CalculateScoreBoard(int questionId, string answer)
        {
            Console.WriteLine($"Incoming questionId: {questionId}");
            
            var participant =  _lobbyParticipantManager.GetLobbyParticipant(Context.ConnectionId);
            
            var question = await _dbContext.QuestionJsonBlobs.FirstOrDefaultAsync(q => q.Id == questionId);
            
            Console.WriteLine(question == null 
                ? "QUESTION NOT FOUND" 
                : "Question found");

            var result = _scoringService.IsCorrect(question, answer, out int points);

            if (result)
            {
                participant.Score += points;
                await Clients.Group(participant.LobbyId.ToString()).SendAsync("ScoreBoardCalculated", question, answer, participant);
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

        public async Task SubmitMultipleAnswers(int lobbyid, int quizId, List<MultipleOptions> participantAnswers, int questionId)
        {
            var participant = _lobbyParticipantManager.GetLobbyParticipant(Context.ConnectionId);
            if (participant != null)
            {
                await Clients.Group(participant.LobbyId.ToString()).SendAsync("MultipleAnswersSubmits", participant, quizId, participantAnswers, questionId);
            }
        }


    }
}